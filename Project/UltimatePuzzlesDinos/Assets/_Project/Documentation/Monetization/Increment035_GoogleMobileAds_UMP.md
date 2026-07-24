# Increment 035 — Google Mobile Ads + UMP

## Important

The integration code is included, but the official Google Mobile Ads Unity Plugin
must be imported separately. The project deliberately does not bundle a third-party
SDK inside this incremental ZIP.

Until the SDK is imported and the `GOOGLE_MOBILE_ADS` symbol is enabled:

- Editor: MockAdsProvider remains available for development flow tests.
- Android build: UnavailableAdsProvider is used and NEVER grants a simulated reward.

## Installation order

1. Import this incremental ZIP into the project root.
2. Let Unity compile. It must compile without Google Mobile Ads installed.
3. Download and import the current supported Google Mobile Ads Unity Plugin v10.x
   from Google's official repository/documentation.
4. Resolve Android dependencies when External Dependency Manager requests it.
5. In Unity run:
   VRM Games > Ultimate Puzzles Dinos > Monetization >
   Enable Google Mobile Ads Integration
6. After recompilation open:
   Assets > Google Mobile Ads > Settings
7. During development use Google's sample Android App ID:
   ca-app-pub-3940256099942544~3347511713
8. Run:
   VRM Games > Ultimate Puzzles Dinos > Monetization >
   Validate Google Mobile Ads

## Test ad units included

Rewarded:
ca-app-pub-3940256099942544/5224354917

Interstitial:
ca-app-pub-3940256099942544/1033173712

These are Google's Android sample ad units and are intentionally enabled by
default.

## Production

Before a release build:

1. Create/register the Free app in AdMob.
2. Create a Rewarded ad unit.
3. Create an Interstitial ad unit.
4. Enter the production App ID in Google Mobile Ads Settings.
5. Open AdMobConfig and fill:
   - productionAndroidAppId
   - productionAndroidRewardedId
   - productionAndroidInterstitialId
6. Disable `Use Google Test Ads`.
7. Validate again.
8. Never use production ad units while testing.

The Premium edition does not initialize or request advertising because AdsEnabled
is false for Premium.

## Child/family configuration

Default configuration:
- Child-directed treatment: enabled.
- UMP under-age-of-consent tag: enabled.
- Maximum ad content rating: G.

For ad requests, child-directed treatment takes precedence and the integration
does not simultaneously set the ad-request TFUA tag when child-directed treatment
is enabled.

UMP is still updated at every launch. With under-age-of-consent enabled, UMP can
determine that consent should not be requested while still keeping privacy state
up to date.

## Privacy options UI

`PrivacyOptionsButton.cs` is provided but is NOT automatically added to any scene,
to preserve the manually adjusted UI.

If `ConsentInformation.PrivacyOptionsRequirementStatus` is Required, add a Button
to Settings and attach `PrivacyOptionsButton`.

## Existing game behavior

Rewarded:
- The reward is granted only from RewardedAd.Show()'s reward callback.
- Closing/failing the ad without earning the reward returns false.
- A new RewardedAd is loaded after use.

Interstitial:
- Existing AdsPolicy cadence/cooldown remains unchanged.
- A new InterstitialAd is loaded after use.
- If no interstitial is ready, gameplay continues without blocking.

Premium:
- No Mobile Ads initialization.
- No rewarded.
- No interstitial.
