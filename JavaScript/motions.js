const leapjs = require('leapjs');

const util = require('./util');

function thumbAbduction(hand) {
    // get directions of thumb and index
    const thumbDirection = hand.thumb.direction;
    const indexDirection = hand.indexFinger.direction;

    // calculate angle between thumb and index
    const angle = util.getAngleBetweenVectors(thumbDirection, indexDirection);
    
    return(`
    hand type: ${hand.type}
    directions:
    \t| thumb: ${thumbDirection}
    \t| index finger: ${indexDirection}
    
    angle thumb - index finger: ${angle}`);
    
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

        return(`
        pinch strength: ${pinchStrength}
        pinching finger: ${util.fingerTypeToNameLookup(pinchingFinger)}
        \t| distance thumb - pinching finger: ${distance}
        `);
    }

    return(`
    no pinching motion detected
    `);
}


module.exports = {
    thumbAbduction,
    thumbOpposition,
}