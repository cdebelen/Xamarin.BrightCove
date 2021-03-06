﻿using System;
using System.Diagnostics;
using AVKit;
using BrightcoveSDK.iOS;
using Foundation;
using UIKit;

namespace Sample.Brightcove.iOS
{
    public class FairPlayViewContoller : UIViewController
    {
        //TODO: change delegates from public to internal?
        public class BCPlaybackControllerDelegate : BCOVPlaybackControllerDelegate
        {
            public override void DidAdvanceToPlaybackSession(BCOVPlaybackController controller, BCOVPlaybackSession session)
            {
                Debug.WriteLine("ViewController Debug - Advanced to new session.");
            }

            public override void PlaybackSessiondidProgressTo(BCOVPlaybackController controller, BCOVPlaybackSession session, double progress)
            {
                Debug.WriteLine($"Progress : {progress} seconds");
            }
        }

        public class BCUIPlaybackViewController : BCOVPUIPlayerViewDelegate
        {
            public override void PictureInPictureControllerDidStartPictureInPicture(AVPictureInPictureController pictureInPictureController)
            {
                Debug.WriteLine("pictureInPictureControllerDidStartPicture");
            }

            public override void PictureInPictureControllerDidStopPictureInPicture(AVPictureInPictureController pictureInPictureController)
            {
                Debug.WriteLine("pictureInPictureControllerDidStopPicture");
            }

            public override void PictureInPictureControllerWillStartPictureInPicture(AVPictureInPictureController pictureInPictureController)
            {
                Debug.WriteLine("pictureInPictureControllerWillStartPicture");
            }

            public override void PictureInPictureControllerWillStopPictureInPicture(AVPictureInPictureController pictureInPictureController)
            {
                Debug.WriteLine("pictureInPictureControllerWillStopPicture");
            }

            public override void PictureInPictureController(AVPictureInPictureController pictureInPictureController, NSError error)
            {
                Debug.WriteLine($"failedToStartPictureInPictureWithError : {error.LocalizedDescription}");
            }
        }

        static string policyKEY = "BCpkADawqM3YRyTQ4hZzmqTk-Oegl3lHc_iLPz29j-aHgdZy0hLaKVj-TlITBvYppMXWpz4mGh60AgWogCIF42vzi1lkj9vgAjYNjAwjd8xeW-JwTb1yI4XPq0mGXaXx4KY-Nu7MwFX0QsQi";
        static string accountID = "6056665239001";
        string videoId = "6061780418001";

        BCOVPlayerSDKManager sDKManager = BCOVPlayerSDKManager.SharedManager();
        BCOVPlaybackService playbackService = new BCOVPlaybackService(accountId: accountID, policyKey: policyKEY);
        BCOVPlaybackController playbackController;

        public FairPlayViewContoller()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //setup fairplay stuffs
            var fairPlayAuthProxy = new BCOVFPSBrightcoveAuthProxy(null, null);

            // Create chain of session providers
            // And upstream session provider to link to. If nil, a BCOVBasicSessionProvider will be used.
            var fps = sDKManager.CreateFairPlaySessionProviderWithAuthorizationProxy(fairPlayAuthProxy, null);

            //Create the playback controller
            playbackController = sDKManager.CreateFairPlayPlaybackControllerWithAuthorizationProxy(fairPlayAuthProxy);
            playbackController.SetAutoPlay(true);
            playbackController.SetAutoAdvance(false);
            playbackController.Delegate = new BCPlaybackControllerDelegate();

            // Set up our player view. Create with a standard VOD layout.
            var options = new BCOVPUIPlayerViewOptions() { ShowPictureInPictureButton = true };
            var playerView = new BCOVPUIPlayerView(playbackController, options, BCOVPUIBasicControlView.BasicControlViewWithVODLayout());
            playerView.Delegate = new BCUIPlaybackViewController();

            playbackService.FindVideoWithVideoID(videoID: videoId, parameters: new NSDictionary(), completionHandler: (arg0, arg1, arg2) =>
            {
                if (arg0 != null)
                {
                    playbackController.SetVideos(NSArray.FromObjects(arg0));
                }
                else
                    Debug.WriteLine($"View Controller Debug - Error retrieving video : {arg2.LocalizedDescription} ");
            });

            playerView.PlaybackController = playbackController;
            playerView.Frame = View.Frame;
            playerView.ControlsView.ProgressSlider.MinimumTrackTintColor = UIColor.Green;
            View.AddSubview(playerView);
        }
    }
}
