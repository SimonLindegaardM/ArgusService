using ArgusService.Interfaces;
using ArgusService.Repositories;

namespace ArgusService.Managers
{
    public class TrackerManager : ITrackerManager
    {
        private readonly TrackerRepository _trackerRepository;
        private readonly UserRepository _userRepository;

        public TrackerManager(TrackerRepository trackerRepository, UserRepository userRepository)
        {
            _trackerRepository = trackerRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Registers a Tracker or Lock device.
        /// </summary>
        public async Task RegisterDeviceAsync(string deviceId, string deviceType, string attachedTrackerId = null)
        {
            if (deviceType.ToLower() != "tracker" && deviceType.ToLower() != "lock")
            {
                throw new ArgumentException("Invalid device type. Allowed types are 'tracker' or 'lock'.");
            }

            await _trackerRepository.RegisterDeviceAsync(deviceId, deviceType, attachedTrackerId);
        }

        /// <summary>
        /// Links a tracker to a specific Firebase user.
        /// </summary>
        public async Task LinkDeviceToUserAsync(string trackerId, string firebaseUID, string email)
        {
            // Validate the user
            var user = await _userRepository.GetUserDetailsAsync(firebaseUID);
            if (user == null || user.Email != email)
            {
                throw new Exception($"User with FirebaseUID {firebaseUID} and email {email} not found.");
            }

            // Fetch the tracker and link it
            var tracker = await _trackerRepository.FetchTrackerAsync(trackerId); 
            if (tracker == null)
            {
                throw new Exception($"Tracker with ID {trackerId} not found.");
            }

            tracker.FirebaseUID = firebaseUID;
            tracker.Email = email;
            await _trackerRepository.UpdateTrackerAsync(tracker);
        }

        /// <summary>
        /// Retrieves all trackers and locks.
        /// </summary>
        public async Task<List<object>> FetchAllDevicesAsync()
        {
            return await _trackerRepository.GetAllDevicesAsync();
        }

        /// <summary>
        /// Updates the lock state for a tracker.
        /// </summary>
        public async Task UpdateLockStateAsync(string trackerId, string lockState)
        {
            if (lockState.ToLower() != "locked" && lockState.ToLower() != "unlocked")
            {
                throw new ArgumentException("Invalid lock state. Allowed states are 'locked' or 'unlocked'.");
            }

            await _trackerRepository.UpdateLockStateAsync(trackerId, lockState);
        }

        /// <summary>
        /// Fetches the current lock state for a tracker.
        /// </summary>
        public async Task<string> FetchLockStateAsync(string trackerId)
        {
            return await _trackerRepository.FetchLockStateAsync(trackerId);
        }
    }
}
