using System;
using System.Collections.ObjectModel;

namespace Record.Recorder.Core
{
    public class TrackPosition
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Number { get; set; }

        public TrackPosition()
        {

        }

        public TrackPosition(int number, TimeSpan start, TimeSpan end)
        {
            Number = number;
            Start = start;
            End = end;
        }
    }

    public class TrackPositionCollection : Collection<TrackPosition>
    {
        public void Add(int number, TimeSpan start, TimeSpan end)
        {
            Add(new TrackPosition(number, start, end));
        }
    }
}
