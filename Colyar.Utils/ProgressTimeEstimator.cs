using System;
using System.Collections.Generic;
using System.Text;

namespace Colyar.Utils
{
    public class ProgressTimeEstimator
    {
        private DateTime _startTime;
        private DateTime _lastTimeMark;
        private int _updateCount;
        private int _updatesRemaining;

        public ProgressTimeEstimator(DateTime startTime, int totalUpdates)
        {
            this._startTime = startTime;
            this._updatesRemaining = totalUpdates;
        }

        public void Update()
        {
            ++this._updateCount;
            --this._updatesRemaining;
        }

        public string GetApproxTimeRemaining()
        {
            if (this._updateCount == 0)
                return "Calculating...";

            TimeSpan timespan = DateTime.Now - this._startTime;

            int averageSeconds = (int)timespan.TotalSeconds/this._updateCount;
            int secondsRemaining = averageSeconds*this._updatesRemaining;

            int minutesRemaining = secondsRemaining/60;
            int hoursRemaining = minutesRemaining/60;
            int daysRemaining = hoursRemaining/24;

            if (daysRemaining > 0)
                return String.Format("About {0} {1} remaining.", daysRemaining, PluralizeIfNeeded("day", daysRemaining)); 
            if (hoursRemaining > 0)
                return String.Format("About {0} {1} remaining.", hoursRemaining, PluralizeIfNeeded("hour", hoursRemaining)); 
            if (minutesRemaining > 0)
                return String.Format("About {0} {1} remaining.", minutesRemaining, PluralizeIfNeeded("minute", minutesRemaining));

            return String.Format("About {0} {1} remaining.", secondsRemaining, PluralizeIfNeeded("second", secondsRemaining)); 
        }

        private string PluralizeIfNeeded(string word, int count)
        {
            if (count == 0 || count > 1)
                return word.TrimEnd("s".ToCharArray()) + "s";

            return word;
        }
    }
}
