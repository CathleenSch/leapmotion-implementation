function roundToFive(number) {
    let range;

    const ranges = [
        [0, 2.5],
        [2.5, 7.5],
        [7.5, 12.5],
        [12.5, 17.5],
        [17.5, 22.5],
        [22.5, 27.5],
        [27.5, 32.5],
        [32.5, 37.5],
        [37.5, 42.5],
        [42.5, 47.5],
        [47.5, 52.5],
        [52.5, 57.5],
        [57.5, 62.5],
        [62.5, 67.5],
        [67.5, 72.5],
        [72.5, 77.5],
        [77.5, 82.5],
        [82.5, 87.5],
        [87.5, 92.5],
        [92.5, 97.5],
        [97.5, 102.5]
    ]

    for (let i = 0; i < ranges.length; i++) {
        if (number >= ranges[i][0] && number <= ranges[i][1]) {
            range = ranges[i];
            break;
        }
    }

    if (!range) {
        return 100;
    }
    return (range[1] - 2.5);
}


function getAngleBetweenVectors(u, v) {
    const uTimesV = u[0] * v[0] + u[1] * v[1] + u[2] * v[2];
    const absU = Math.sqrt(Math.pow(u[0], 2) + Math.pow(u[1], 2) + Math.pow(u[2], 2));
    const absV = Math.sqrt(Math.pow(v[0], 2) + Math.pow(v[1], 2) + Math.pow(v[2], 2));
    const cosinus = uTimesV / (absU * absV);
    const angleDegrees = (Math.acos(cosinus) * 180 / Math.PI).toFixed(1);

    return roundToFive(angleDegrees);
}

module.exports = {
    roundToFive,
    getAngleBetweenVectors,
}