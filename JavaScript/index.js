const fs = require('fs');
const readline = require('readline');
const leapjs = require('leapjs');

const motions = require('./motions');

const mode = process.argv[2];

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
  });

let stream;
let startTime;
let recordingTime;

if (mode === 'abd') {
    recordingTime = 7000;
    fs.unlink('abd.txt', () => {
        stream = fs.createWriteStream('abd.txt', {flags: 'a'});
    });
} else if (mode === 'opp') {
    recordingTime = 15000;
    fs.unlink('opp.txt', () => {
        stream = fs.createWriteStream('opp.txt', {flags: 'a'});
    });
} else if (mode === 'wf') {
    recordingTime = 7000;
    fs.unlink('wf.txt', () => {
        stream = fs.createWriteStream('wf.txt', {flags: 'a'});
    });
} else {
    recordingTime = 7000;
    fs.unlink('we.txt', () => {
        stream = fs.createWriteStream('we.txt', {flags: 'a'});
    });
}

rl.question('Press any key to start recording.', (key) => {
    setTimeout(() => {
        startTime = Date.now();
        startLeapMotionService();
    }, 1000);
});


function startLeapMotionService() {
    const controller = new leapjs.Controller();


    controller.on('connect', () => {
        console.log('successfully connected');
    });

    controller.on('deviceStreaming', () => {
        console.log('device connected\n\n');
    });

    controller.on('deviceStopped', () => {
        console.log('device disconnected');
    });

    controller.on('deviceFrame', (frame) => {
        const timeElapsed = Date.now() - startTime;
        // const timestamp = Math.floor(timeElapsed / 10);
        console.clear();
        let output = ``;
        // do all the stuff here
        if (frame.hands.length > 0) {
            const hand = frame.hands[0];

            if (mode === 'abd') {
                const result = motions.thumbAbduction(hand);
                stream.write(`timestamp: ${timeElapsed} | hand: ${hand.type} | angle: ${result.angle}\n`);

                output = result.output;
            }

            if (mode === 'opp') {
                const result = motions.thumbOpposition(hand);
                console.log(typeof result.pinchStrength);
                if (typeof result.pinchStrength === 'number') {
                    stream.write(`timestamp: ${timeElapsed} | hand: ${hand.type} | pinch strength: ${result.pinchStrength} | finger: ${result.fingerName} | distance: ${result.closest}\n`);
                }

                output = result.output;
            }

            if (mode === 'wf') {
                const result = motions.wristFlexionExtension(hand);
                stream.write(`timestamp: ${timeElapsed} | hand: ${hand.type} | angle: ${result.angle}\n`)

                output = result.output;
            }

            if (mode === 'we') {
                const result = motions.wristFlexionExtension(hand);
                stream.write(`timestamp: ${timeElapsed} | hand: ${hand.type} | angle: ${result.angle}\n`)

                output = result.output;
            }
        } else {
            output = `no hand detected`;
        }

        console.log(output);
    })

    controller.connect();

    setTimeout(() => {
        controller.disconnect();
        console.log('controller disconnected');
    }, recordingTime);
}