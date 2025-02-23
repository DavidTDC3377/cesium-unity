using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using CesiumForUnity;
using NUnit.Framework;
using UnityEngine.TestTools.Utils;

public class TestCesiumGlobeAnchor
{
    [UnityTest]
    public IEnumerator SyncsUnityPositionFromTransformAndBackMultipleFrames()
    {
        GameObject goGeoreference = new GameObject("Georeference");
        CesiumGeoreference georeference = goGeoreference.AddComponent<CesiumGeoreference>();
        georeference.longitude = -55.0;
        georeference.latitude = 55.0;
        georeference.height = 1000.0;

        GameObject goAnchored = new GameObject("Anchored");
        goAnchored.transform.parent = goGeoreference.transform;
        goAnchored.transform.SetPositionAndRotation(new Vector3(100.0f, 200.0f, 300.0f), Quaternion.Euler(10.0f, 20.0f, 30.0f));

        CesiumGlobeAnchor anchor = goAnchored.AddComponent<CesiumGlobeAnchor>();
        Assert.AreEqual(CesiumGlobeAnchorPositionAuthority.None, anchor.positionAuthority);

        // Wait for the start of a new frame, which will cause Start to be invoked.
        yield return null;

        Assert.AreEqual(CesiumGlobeAnchorPositionAuthority.UnityCoordinates, anchor.positionAuthority);
        Assert.That(anchor.unityX, Is.EqualTo(100.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityY, Is.EqualTo(200.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityZ, Is.EqualTo(300.0).Using(FloatEqualityComparer.Instance));

        anchor.SetPositionUnity(1.0, 2.0, 3.0);

        Assert.That(anchor.unityX, Is.EqualTo(1.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityY, Is.EqualTo(2.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityZ, Is.EqualTo(3.0).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.position.x, Is.EqualTo(1.0).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.position.y, Is.EqualTo(2.0).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.position.z, Is.EqualTo(3.0).Using(FloatEqualityComparer.Instance));
    }

    [Test]
    public void SyncsUnityPositionFromTransformAndBackSingleFrame()
    {
        GameObject goGeoreference = new GameObject("Georeference");
        CesiumGeoreference georeference = goGeoreference.AddComponent<CesiumGeoreference>();
        georeference.longitude = -55.0;
        georeference.latitude = 55.0;
        georeference.height = 1000.0;

        GameObject goAnchored = new GameObject("Anchored");
        goAnchored.transform.parent = goGeoreference.transform;
        goAnchored.transform.SetPositionAndRotation(new Vector3(100.0f, 200.0f, 300.0f), Quaternion.Euler(10.0f, 20.0f, 30.0f));

        CesiumGlobeAnchor anchor = goAnchored.AddComponent<CesiumGlobeAnchor>();

        // Manually update the globe anchor properties without waiting for Unity to invoke Start on it.
        anchor.Sync();

        Assert.AreEqual(CesiumGlobeAnchorPositionAuthority.UnityCoordinates, anchor.positionAuthority);
        Assert.That(anchor.unityX, Is.EqualTo(100.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityY, Is.EqualTo(200.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityZ, Is.EqualTo(300.0).Using(FloatEqualityComparer.Instance));

        anchor.SetPositionUnity(1.0, 2.0, 3.0);

        Assert.That(anchor.unityX, Is.EqualTo(1.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityY, Is.EqualTo(2.0).Using(FloatEqualityComparer.Instance));
        Assert.That(anchor.unityZ, Is.EqualTo(3.0).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.position.x, Is.EqualTo(1.0).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.position.y, Is.EqualTo(2.0).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.position.z, Is.EqualTo(3.0).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator StartDoesNotClobberPreviouslySetPosition()
    {
        GameObject goGeoreference = new GameObject("Georeference");
        CesiumGeoreference georeference = goGeoreference.AddComponent<CesiumGeoreference>();
        georeference.longitude = -55.0;
        georeference.latitude = 55.0;
        georeference.height = 1000.0;

        GameObject goAnchored = new GameObject("Anchored");
        goAnchored.transform.parent = goGeoreference.transform;

        CesiumGlobeAnchor anchor = goAnchored.AddComponent<CesiumGlobeAnchor>();
        anchor.SetPositionLongitudeLatitudeHeight(45.0, -45.0, 101.0);
        Assert.AreEqual(CesiumGlobeAnchorPositionAuthority.LongitudeLatitudeHeight, anchor.positionAuthority);
        Assert.AreEqual(45.0, anchor.longitude);
        Assert.AreEqual(-45.0, anchor.latitude);
        Assert.AreEqual(101.0, anchor.height);

        yield return null;

        Assert.AreEqual(CesiumGlobeAnchorPositionAuthority.LongitudeLatitudeHeight, anchor.positionAuthority);
        Assert.AreEqual(45.0, anchor.longitude);
        Assert.AreEqual(-45.0, anchor.latitude);
        Assert.AreEqual(101.0, anchor.height);
    }

    [UnityTest]
    public IEnumerator SettingPositionImmediatelyAfterAddingAnchorDoesNotAffectOrientation()
    {
        GameObject goGeoreference = new GameObject("Georeference");
        CesiumGeoreference georeference = goGeoreference.AddComponent<CesiumGeoreference>();
        georeference.longitude = -55.0;
        georeference.latitude = 55.0;
        georeference.height = 1000.0;

        GameObject goAnchored = new GameObject("Anchored");
        goAnchored.transform.parent = goGeoreference.transform;
        goAnchored.transform.SetPositionAndRotation(new Vector3(100.0f, 200.0f, 300.0f), Quaternion.Euler(10.0f, 20.0f, 30.0f));

        Assert.That(goAnchored.transform.rotation.eulerAngles.x, Is.EqualTo(10.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.y, Is.EqualTo(20.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.z, Is.EqualTo(30.0f).Using(FloatEqualityComparer.Instance));

        CesiumGlobeAnchor anchor = goAnchored.AddComponent<CesiumGlobeAnchor>();

        // Move to the opposite side of the globe in terms of longitude.
        anchor.SetPositionLongitudeLatitudeHeight(125.0, 55.0, 1000.0);

        // The orientation should be unaffected because the globe anchor hasn't been Sync'd yet.
        Assert.That(goAnchored.transform.rotation.eulerAngles.x, Is.EqualTo(10.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.y, Is.EqualTo(20.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.z, Is.EqualTo(30.0f).Using(FloatEqualityComparer.Instance));

        //// Wait for the start of a new frame, which will cause Start to be invoked.
        yield return null;

        // The orientation should still be unaffected
        Assert.That(goAnchored.transform.rotation.eulerAngles.x, Is.EqualTo(10.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.y, Is.EqualTo(20.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.z, Is.EqualTo(30.0f).Using(FloatEqualityComparer.Instance));

        // But now if we move it, the orientation will change, too.
        anchor.SetPositionLongitudeLatitudeHeight(105.0, 55.0, 1000.0);
        Assert.That(goAnchored.transform.rotation.eulerAngles.x, Is.Not.EqualTo(10.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.y, Is.Not.EqualTo(20.0f).Using(FloatEqualityComparer.Instance));
        Assert.That(goAnchored.transform.rotation.eulerAngles.z, Is.Not.EqualTo(30.0f).Using(FloatEqualityComparer.Instance));
    }
}
