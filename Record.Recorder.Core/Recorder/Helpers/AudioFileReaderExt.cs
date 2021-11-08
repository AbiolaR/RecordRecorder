using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Record.Recorder.Core
{
    static class AudioFileReaderExt
    {
        public static TrackPositionCollection GetTrackPositions(this AudioFileReader reader, sbyte silenceThreshold = -40)
        {
            Dictionary<string, TimeSpan> silencePositions = GetSilencePositions(reader, silenceThreshold);
            var trackPositions = new TrackPositionCollection();
            int i = 1, silencePositionsAmount = silencePositions.Count / 2;

            silencePositions.TryGetValue("Start: 1", out TimeSpan firstStart);

            if (firstStart.TotalMilliseconds != 0)
            {
                trackPositions.Add(i, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(firstStart.TotalMilliseconds - 1));
                i++;
            }

            for (; i <= silencePositionsAmount; i++)
            {
                silencePositions.TryGetValue("End: " + i, out TimeSpan end);
                silencePositions.TryGetValue("Start: " + (i + 1), out TimeSpan nextStart);

                if (end.Equals(reader.TotalTime)) break;

                var trackPosition = new TrackPosition { Number = i, Start = TimeSpan.FromMilliseconds(end.TotalMilliseconds + 1) };
                if (nextStart.TotalMilliseconds == 0)
                {
                    trackPosition.End = reader.TotalTime;
                    trackPositions.Add(trackPosition);
                }
                else
                {
                    trackPosition.End = TimeSpan.FromMilliseconds(nextStart.TotalMilliseconds - 1);
                    trackPositions.Add(trackPosition);
                }
            }

            return trackPositions;
        }

        private static bool IsSilence(float amplitude, sbyte threshold)
        {
            double dB = 20 * Math.Log10(Math.Abs(amplitude));
            return dB < threshold;
        }

        private static void ParallelWhile(Func<bool> condition, System.Action body)
        {
            Parallel.ForEach(IterateUntilFalse(condition), ignored => body());
        }

        private static IEnumerable<bool> IterateUntilFalse(Func<bool> condition)
        {
            while (condition()) yield return true;
        }

        private static void StorePossibleSilenceEndPosition(this AudioFileReader reader, ArrayList possibleSilencePositions, int silenceStart, int counterSilence)
        {
            double silenceSamples;
            double silenceDuration;
            double silenceEnd;

            silenceSamples = (double)silenceStart / reader.WaveFormat.Channels;
            silenceDuration = silenceSamples / reader.WaveFormat.SampleRate * 1000;

            possibleSilencePositions.Add(TimeSpan.FromMilliseconds(silenceDuration));

            silenceEnd = silenceStart + counterSilence;
            silenceSamples = silenceEnd / reader.WaveFormat.Channels;
            silenceDuration = silenceSamples / reader.WaveFormat.SampleRate * 1000;

            possibleSilencePositions.Add(TimeSpan.FromMilliseconds(silenceDuration));
        }

        private static Dictionary<string, TimeSpan> GetSilencePositions(this AudioFileReader reader, sbyte silenceThreshold = -40)
        {

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
                            StorePossibleSilenceEndPosition(reader, possibleSilencePositions, silenceStart, counterSilence);

                            counterSilence = 0;
                            silenceFound = false;
                        }
                    }

                    counter++;
                }
            }

            if (silenceFound)
            {
                StorePossibleSilenceEndPosition(reader, possibleSilencePositions, silenceStart, counterSilence);
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

                if (difference.TotalSeconds > 2 || start.TotalMilliseconds == 0)
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
