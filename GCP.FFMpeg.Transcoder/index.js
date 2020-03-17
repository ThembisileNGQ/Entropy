const storage = require('@google-cloud/storage')();
const ffmpegPath = require('@ffmpeg-installer/ffmpeg').path;
const ffmpeg = require('fluent-ffmpeg');
const transcodedBucket = storage.bucket('processedtracks');
const uploadBucket = storage.bucket('uploadedtracks');
ffmpeg.setFfmpegPath(ffmpegPath);

function guid4() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

exports.transcodeAudio = function transcodeAudio(event, callback) {
  const file = event.data;
  
  console.log("event");
  console.log(event);
  console.log("ffmpeg path");
  console.log(ffmpegPath);
  
  // Ensure that you only proceed if the file is newly created, and exists.
  if (file.metageneration !== '1') {
    console.log('file is not new');
    callback();
    return;
  }
  
  // Ensure that you only proceed if the file is newly created, and exists.
  if (file.resourceState !== 'exists') {
    //console.log(file);
    //console.log('file does not exist');
    //callback();
    //return;
  }

  // Open write stream to new bucket, modify the filename as needed.
  var guid = guid();
  var fileName = guid + '.mp3'
  const remoteWriteStream = transcodedBucket.file(fileName)
    .createWriteStream({
      metadata: {
        metadata: file.metadata, // You may not need this, my uploads have associated metadata
        contentType: 'audio/mpeg', // This could be whatever else you are transcoding to
      },
    });

  // Open read stream to our uploaded file
  const remoteReadStream = uploadBucket.file(file.name).createReadStream();

  
  // Transcode
  ffmpeg()
    .input(remoteReadStream)
    // set audio bitrate
    .audioBitrate('128k')
    // set audio codec
    .audioCodec('libmp3lame')
    // set number of audio channels
    .audioChannels(2)
    // set output format to force
    .format('mp3')
    // setup event handlers
    .on('start', (cmdLine) => {
      console.log('Started ffmpeg with command:', cmdLine);
    })
    // setup event handlers
    .on('end', () => {
      console.log('Successfully re-encoded audio.');
      callback();
    })
    .on('error', (err, stdout, stderr) => {
      console.error('An error occured during encoding', err.message);
      console.error('stdout:', stdout);
      console.error('stderr:', stderr);
      callback(err);
    })
    .pipe(remoteWriteStream, { end: true }); // end: true, emit end event when readable stream ends
};
