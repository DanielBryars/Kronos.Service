using System;

namespace Kronos.Framework.Schedule
{
    public class ScheduledBoolean
    {
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private bool _outputInverted;
        private string _name;
        private string _description;
        private bool _lastOutput = false;

        /// <summary>
        /// startTime and endTime are TimeSpan's since Midnight, a time span of 00:06:00 is 6 hours after midnight (more commonly known as 6 am)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outputInverted"></param>
        public ScheduledBoolean(string name, string description, TimeSpan startTime, TimeSpan endTime, bool outputInverted = false)
        {
            if (endTime < startTime) throw new ArgumentException($"startTime:'{startTime}' must be less than endTime:'{endTime}'");

            _name = name;
            _description = description;
            _startTime = startTime;
            _endTime = endTime;
            _outputInverted = outputInverted;
        }

        public string Name { get { return _name; } }

        public (bool currentValue, bool lastValue) GetOutput(DateTimeOffset timeNow)
        {
            var result = timeNow.TimeOfDay > _startTime && timeNow.TimeOfDay < _endTime;
            var currentOutput = result ^ _outputInverted;
            var lastOutput = _lastOutput;
            _lastOutput = currentOutput;

            return (currentOutput, lastOutput);
        }

        public override string ToString()
        {
            return $"{_name}:{_startTime} => {_endTime}";
        }
    }
}
