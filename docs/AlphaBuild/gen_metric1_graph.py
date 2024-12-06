import matplotlib.pyplot as plt

data = [
    [9.36, 12.20, 13.42, 19.62, 21.35],
    [14.56, 15.91, 17.42, 19.13, 20.11],
    [31.5, 49.33, 51.90, 58.13, 61.56],
    [18.66, 25.72, 27.14, 35.04, 38.91]
]

x_labels = ['Level1', 'Level2', 'Level3', 'Level4']

avg_time = [sum(data[i])/len(data[i]) for i in range(4)]

plt.figure(figsize=(10, 5))
plt.bar(x_labels, avg_time, color='blue')
plt.title('Average Finished Time for Each Level')
plt.xlabel('Level')
plt.ylabel('Time (s)')

plt.savefig('average_time.png')
