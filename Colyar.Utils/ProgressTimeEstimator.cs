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
        private int _totalUpdates;

        public ProgressTimeEstimator(DateTime startTime, int totalUpdates)
        {
            this._startTime = startTime;
            this._totalUpdates = totalUpdates;
        }

        public void Update()
        {
            ++this._updateCount;
        }

        public string GetApproxTimeRemaining()
        {
            if (this._updateCount == 0)
                return "Calculating...";

            if (this._updateCount == this._totalUpdates)
                return "Done.";

            TimeSpan timespan = DateTime.Now - this._startTime;
            int updatesRemaining = this._totalUpdates - this._updateCount;

            double averageSeconds = timespan.TotalSeconds/this._updateCount;
            double secondsRemaining = averageSeconds * updatesRemaining;

            double minutesRemaining = secondsRemaining / 60.0;
            double hoursRemaining = minutesRemaining / 60.0;
            double daysRemaining = hoursRemaining / 24.0;

            daysRemaining = Math.Round(daysRemaining);
            hoursRemaining = Math.Round(hoursRemaining);
            minutesRemaining = Math.Round(minutesRemaining);

            if (daysRemaining > 0)
                return String.Format("About {0} {1} remaining.", daysRemaining, PluralizeIfNeeded("day", daysRemaining)); 
            if (hoursRemaining > 0)
                return String.Format("About {0} {1} remaining.", hoursRemaining, PluralizeIfNeeded("hour", hoursRemaining)); 
            if (minutesRemaining > 0)
                return String.Format("About {0} {1} remaining.", minutesRemaining, PluralizeIfNeeded("minute", minutesRemaining));

            return "Less than a minute remaining.";
        }

        private string PluralizeIfNeeded(string word, double count)
        {
            if (count == 0 || count > 1)
                return word.TrimEnd("s".ToCharArray()) + "s";

            return word;
        }
    }
}
