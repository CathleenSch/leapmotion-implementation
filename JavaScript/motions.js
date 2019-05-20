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


module.exports = {
    thumbAbduction,
}