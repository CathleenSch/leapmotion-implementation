const leapjs = require('leapjs');

const util = require('./util');

function thumbAbduction(hand) {
    // get directions of thumb and index
    const thumbDirection = hand.thumb.direction;
    const indexDirection = hand.indexFinger.direction;

    // calculate angle between thumb and index
    const angle = util.getAngleBetweenVectors(thumbDirection, indexDirection);
    
    const output = `
    hand type: ${hand.type}
    directions:
    \t| thumb: ${thumbDirection}
    \t| index finger: ${indexDirection}
    
    angle thumb - index finger: ${angle}`;

    return{
        angle,
        output,
    };
    
}


function thumbOpposition(hand) {
    const pinchStrength = hand.pinchStrength;
    let pinchingFinger;
    let distance;

    if (pinchStrength > 0) {
        let closest = 500;
        let currentFinger;

        for (let i = 1; i < hand.fingers.length; i++) {
            currentFinger = hand.fingers[i];
            distance = leapjs.vec3.distance(hand.thumb.tipPosition, currentFinger.tipPosition);

            if (distance < closest) {
                closest = distance;
                pinchingFinger = currentFinger;
            }
        }

        const fingerName = util.fingerTypeToNameLookup(pinchingFinger);

        const output = `
        hand type: ${hand.type}
        pinch strength: ${pinchStrength}
        pinching finger: ${fingerName}
        \t| distance thumb - pinching finger: ${distance}
        `;

        return{
            pinchStrength,
            fingerName,
            closest,
            output,
        };
    }

    return(`
    no pinching motion detected
    `);
}


function wristFlexionExtension(hand) {
    const arm = hand.arm;
    const armDirection = arm.direction();
    const handDirection = hand.direction;

    const angle = util.getAngleBetweenVectors(armDirection, handDirection);
    const output = `
    hand type: ${hand.type}
    directions:
    \t| arm: ${armDirection}
    \t| hand: ${handDirection}
    
    angle: ${angle}
    `;

    return {
        angle,
        output,
    };
}


module.exports = {
    thumbAbduction,
    thumbOpposition,
    wristFlexionExtension,
}