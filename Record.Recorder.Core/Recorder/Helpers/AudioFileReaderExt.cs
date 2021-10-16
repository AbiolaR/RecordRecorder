﻿using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Record.Recorder.Core
{
    static class AudioFileReaderExt
    {
        public static Dictionary<string, TimeSpan> GetTrackPositions(this AudioFileReader reader, sbyte silenceThreshold = -40)
        {
            Dictionary<string, TimeSpan> silencePositions = GetSilencePositions(reader, silenceThreshold);
            Dictionary<string, TimeSpan> trackPositions = new Dictionary<string, TimeSpan>();
            int i = 1, silencePositionsAmount = silencePositions.Count / 2;

            silencePositions.TryGetValue("Start: 1", out TimeSpan firstStart);

            if (firstStart.TotalMilliseconds != 0)
            {
                trackPositions.Add("Start: 1", TimeSpan.FromMilliseconds(0));
                trackPositions.Add("End: 1", TimeSpan.FromMilliseconds(firstStart.TotalMilliseconds - 1));
                i++;
            }

            for (; i <= silencePositionsAmount; i++)
            {
                silencePositions.TryGetValue("End: " + i, out TimeSpan end);
                silencePositions.TryGetValue("Start: " + (i + 1), out TimeSpan nextStart);

                trackPositions.Add("Start: " + i, TimeSpan.FromMilliseconds(end.TotalMilliseconds + 1));
                if (nextStart.TotalMilliseconds == 0)
                {
                    trackPositions.Add("End: " + i, reader.TotalTime);
                }
                else
                {
                    trackPositions.Add("End: " + i, TimeSpan.FromMilliseconds(nextStart.TotalMilliseconds - 1));
                }
            }

            return trackPositions;
        }

        private static bool IsSilence(float amplitude, sbyte threshold)
        {
            double dB = 20 * Math.Log10(Math.Abs(amplitude));
            return dB < threshold;
        }
        private static Dictionary<string, TimeSpan> GetSilencePositions(this AudioFileReader reader, sbyte silenceThreshold = -40)
        {
            double silenceSamples;
            double silenceDuration;
            double silenceEnd;
            long oldPosition = reader.Position;
            ArrayList possibleSilencePositions = new ArrayList();
            int counter = 0, counterSilence = 0, silenceStart = 0;
            bool eof = false, silenceFound = false;

            var buffer = new float[reader.WaveFormat.SampleRate * 4];
            while (!eof)
            {
                int samplesRead = reader.Read(buffer, 0, buffer.Length);
                if (samplesRead == 0)
                    eof = true;

                for (int n = 0; n < samplesRead; n++)
                {
                    if (IsSilence(buffer[n], silenceThreshold))
                    {

                        if (!silenceFound) silenceStart = counter;
                        counterSilence++;
                        silenceFound = true;
                    }
                    else
                    {
                        if (silenceFound)
                        {
                            silenceSamples = (double)silenceStart / reader.WaveFormat.Channels;
                            silenceDuration = (silenceSamples / reader.WaveFormat.SampleRate) * 1000;

                            possibleSilencePositions.Add(TimeSpan.FromMilliseconds(silenceDuration));

                            silenceEnd = silenceStart + counterSilence;
                            silenceSamples = silenceEnd / reader.WaveFormat.Channels;
                            silenceDuration = (silenceSamples / reader.WaveFormat.SampleRate) * 1000;

                            possibleSilencePositions.Add(TimeSpan.FromMilliseconds(silenceDuration));
                            counterSilence = 0;
                            silenceFound = false;
                        }
                    }

                    counter++;
                }
            }

            // reset position
            reader.Position = oldPosition;

            TimeSpan start;
            TimeSpan end;
            Dictionary<string, TimeSpan> silencePositions = new Dictionary<string, TimeSpan>();

            for (int i = 0, j = 1; i < possibleSilencePositions.Count; i++, i++)
            {
                start = (TimeSpan)possibleSilencePositions[i];
                end = (TimeSpan)possibleSilencePositions[i + 1];

                TimeSpan difference = end.Subtract(start);

                if (difference.TotalSeconds > 4 || start.TotalMilliseconds == 0)
                {
                    silencePositions.Add("Start: " + j, start);
                    silencePositions.Add("End: " + j, end);
                    j++;
                }
            }

            return silencePositions;
        }
        
    }
}