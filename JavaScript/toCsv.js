const fs = require('fs');
const rl = require('readline');

const file = process.argv[2];

const lineReader = rl.createInterface({
    input: fs.createReadStream(`../testData/${file}.txt`)
});

fs.writeFile(`../CSV/${file}.csv`, '');
const stream = fs.createWriteStream(`../CSV/${file}.csv`, {flags: 'a'});
let firstLine = true;

lineReader.on('line', (line) => {
    console.log(`read: ${line}`);

    if (file.indexOf('C#') > -1) {
        line = line.replace(/,/g, '.');
    }

    let separated = line.split(' | ').join(';').split(': ').join(';').split(';');
    if (firstLine === true) {
        let header = [];
        let writeLine = [];

        for (let i = 0; i < separated.length; i++) {
            if (i%2 === 0) {
                header.push(separated[i]);
            } else {
                writeLine.push(separated[i]);
            }
        }

        header = header.join(',');
        writeLine = writeLine.join(',');

        stream.write(`${header}\n`);
        stream.write(`${writeLine}\n`);

        firstLine = false;
    } else {
        let writeLine = [];

        for (let i = 0; i < separated.length; i++) {
            if (i%2 != 0) {
                writeLine.push(separated[i]);
            }
        }

        stream.write(`${writeLine}\n`);
    }
});

