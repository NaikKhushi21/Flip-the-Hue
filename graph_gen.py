import matplotlib.pyplot as plt
import numpy as np
import matplotlib.image as mpimg
import firebase_admin
from firebase_admin import credentials, db
import collections
from matplotlib.colors import LinearSegmentedColormap

levels_size = {'unity_min_x': [51.94-75, -0.3-129.15/2, -60], 'unity_max_x': [51.94+75, -0.3+129.15/2, 60], 'unity_min_y': [1.56-7, 1.56-7, 1.56-7], 'unity_max_y': [1.56+7, 1.56+7, 1.56+7]}

def sketch_bar_charts(data, title, x_label, y_label, filename):

    x_labels = ['Level1', 'Level2', 'Level3']

    if title == 'trapResets':
        avg_val = [sum(data[i])/len(data[i]) * 10 for i in range(len(x_labels))]
    else:
        avg_val = [sum(data[i])/len(data[i]) for i in range(len(x_labels))]


    plt.figure(figsize=(10, 5))
    plt.bar(x_labels, avg_val, color='skyblue')
    plt.title(title)
    # plt.xlabel(x_label) # Level
    plt.ylabel(y_label) # Time (s)
    # plt.show()
    plt.savefig(f'images/bar_chart_' + filename) # metric_1.png

def sketch_heatmap(unity_positions, level, title):

    # Load your background image
    background_image_path = f'Level{level+1}.png'
    background = mpimg.imread(background_image_path)
    image_height, image_width = background.shape[:2]


    # Convert Unity coordinates to normalized image coordinates and prepare for plotting
    x_values = []
    y_values = []
    colors = []

    unity_min_x, unity_max_x = levels_size['unity_min_x'][level], levels_size['unity_max_x'][level]
    unity_min_y, unity_max_y = levels_size['unity_min_y'][level], levels_size['unity_max_y'][level]


    color = ['yellow', 'red']
    cmap = LinearSegmentedColormap.from_list("custom_yellow_red", color, N=256)

    for x, y, count in unity_positions:
        if not (unity_min_x < x < unity_max_x and unity_min_y < y < unity_max_y):
            continue
        # print(x, y)
        pixel_x = (x - unity_min_x) / (unity_max_x - unity_min_x) * image_width
        pixel_y = (y - unity_min_y) / (unity_max_y - unity_min_y) * image_height
        x_values.append(pixel_x)
        y_values.append(pixel_y)  # Adjust for coordinate flip in image display
        colors.append(count)

    # Create the plot with the background image
    plt.figure(figsize=(10, 5))
    plt.imshow(background, extent=[0, image_width, 0, image_height])  # Set the extent to match the image size
    sc = plt.scatter(x_values, y_values, c=colors, cmap=cmap, s=50, alpha=0.5)  # Use alpha for better visibility
    plt.colorbar(sc, label='Death Count')
    plt.title(f'level{level+1} ' + title)
    plt.xlabel('X Position')
    plt.ylabel('Y Position')
    plt.axis('on')  # Optionally turn off the axis if not needed

    # plt.show()
    plt.savefig(f'images/heatmap_level{level+1}' + title) # metric_1.png


cred = credentials.Certificate("flipthehue-firebase-adminsdk-m6fgg-3fd803f33c.json")
firebase_admin.initialize_app(cred, {
    'databaseURL': 'https://flipthehue-default-rtdb.firebaseio.com'
})

bar_chart_stats = collections.defaultdict(list)
heatmap_stats = collections.defaultdict(list)


def fetch_player_metrics():
    ref = db.reference('/PlayerMetrics')
    player_metrics = ref.get()
    
    # Iterate through each guest and their sessions

    for guest_id, sessions in player_metrics.items():
        guest_type = guest_id.split('_')[0]
        if guest_type != 'NewGuestTest':
            continue
        for session in sessions.values():
            for stats in session:
                if not stats: continue
                for level, stat in stats.items():
                    level_num = int(level.split('_')[1])
                    for stat_name, stat_value in stat.items():
                        if stat_name in ['dashPositions', 'swapPositions', 'deathPositions']:
                            heatmap_stats[(level_num, stat_name)].append((stat_value))
                        else:
                            if stat_name == 'levelTime' and stat_value == '0': continue
                            bar_chart_stats[(level_num, stat_name)].append(float(stat_value))
    bar_data = collections.defaultdict(lambda: [[] for _ in range(3)])
    for (level_num, stat_name), val in bar_chart_stats.items():
        bar_data[stat_name][level_num-1] += val
    for stat_name, data in bar_data.items():
        sketch_bar_charts(data, stat_name, 'Level', '', stat_name)
    heatmap_data = collections.defaultdict(lambda: [[] for _ in range(3)])
    for (level_num, stat_name), positions in heatmap_stats.items():
        res = []
        for sub_positions in positions:
            for pos in sub_positions:    
                res.append((round(pos['x']), round(pos['y']) ))
        
        heatmap_data[stat_name][level_num-1] += res

    for key, positions in heatmap_data.items():
        for level, lv_pos in enumerate(positions):
            counter = collections.Counter(lv_pos)
            res = []
            for (x, y), count in counter.items():
                res.append((x, y, count))
            sketch_heatmap(res, level, key)

fetch_player_metrics()


