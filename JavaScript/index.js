const leapjs = require('leapjs');

const motions = require('./motions');

const controller = new leapjs.Controller();
const out = process.stdout;

const mode = process.argv[2];

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
    console.clear();
    let output = ``;
    // do all the stuff here
    if (frame.hands.length > 0) {
        const hand = frame.hands[0];

        const thumb = hand.thumb;
        const index = hand.indexFinger;
        const middle = hand.middleFinger;
        const ring = hand.ringFinger;
        const pinky = hand.pinky;

        if (mode === 'abd') {
            output = motions.thumbAbduction(hand);
        }

        if (mode === 'opp') {
            output = motions.thumbOpposition(hand);
        }
    } else {
        output = `no hand detected`;
    }

    console.log(output);
})

controller.connect();