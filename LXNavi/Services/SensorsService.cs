using System.Numerics;
using LXNavi.Core;

namespace LXNavi;

public class SensorsService
{
    #region Public Constructors

    public SensorsService()
    {
        AccelerometerViewModel = new(nameof(Accelerometer), "m/s^2", Accelerometer);
        GyroscopeViewModel = new(nameof(Gyroscope), "rad/s", Gyroscope);
        MagnetometerViewModel = new(nameof(Magnetometer), "μT", Magnetometer);
    }

    #endregion Public Constructors

    #region Public Events

    public event EventHandler<IntegratedDatasChangedEventArgs> IntegratedDatasChanged;

    #endregion Public Events

    #region Public Properties

    public Sensor Accelerometer { get; } = new();

    public Sensor Gyroscope { get; } = new();

    public Sensor Magnetometer { get; } = new();

    public List<(DateTime UtcTime, EulerAngles EulerAngle)> AttitudeDatas { get; } = new();

    public SensorViewModel AccelerometerViewModel { get; }

    public SensorViewModel GyroscopeViewModel { get; }

    public SensorViewModel MagnetometerViewModel { get; }

    public List<(DateTime UtcTime, Vector3 AccelerometerData, Vector3 GyroscopeData, Vector3 MagnetometerData, EulerAngles EulerAngle)> IntegratedDatas { get; } = new();

    #endregion Public Properties

    #region Public Methods

    public async void SaveAndShareSensorDatas()
    {
        var cacheDirectory = FileSystem.CacheDirectory;
        var fileName = $"sensors_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var filePath = Path.Combine(cacheDirectory, fileName);
        using var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
        using var streamWriter = new StreamWriter(fileStream);
        streamWriter.WriteLine($"time,accX({AccelerometerViewModel.Unit}),accY({AccelerometerViewModel.Unit}),accZ({AccelerometerViewModel.Unit}),gyroX({GyroscopeViewModel.Unit}),gyroY({GyroscopeViewModel.Unit}),gyroZ({GyroscopeViewModel.Unit}),magX({MagnetometerViewModel.Unit}),magY({MagnetometerViewModel.Unit}),magZ({MagnetometerViewModel.Unit}),yaw(deg),pitch(deg),roll(deg)");
        foreach (var data in IntegratedDatas)
        {
            var accel = data.AccelerometerData;
            var gyro = data.GyroscopeData;
            var mag = data.MagnetometerData;
            var angle = data.EulerAngle;
            streamWriter.WriteLine(string.Join(',', data.UtcTime.ToString("yyyy/MM/dd HH:mm:ss.ffffff"), accel.X, accel.Y, accel.Z, gyro.X, gyro.Y, gyro.Z, mag.X, mag.Y, mag.Z, angle.Yaw.Degrees, angle.Pitch.Degrees, angle.Roll.Degrees));
        }
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share sensor data file",
            File = new ShareFile(filePath)
        });
    }

    #endregion Public Methods

    #region Public Classes

    public class IntegratedDatasChangedEventArgs : EventArgs
    {
        #region Public Constructors

        public IntegratedDatasChangedEventArgs(DeadReckoningMeasurement measurement)
        {
            Measurement = measurement;
        }

        #endregion Public Constructors

        #region Public Properties

        public DeadReckoningMeasurement Measurement { get; init; }

        #endregion Public Properties
    }

    #endregion Public Classes

    #region Private Fields

    private readonly TimeSpan _maximumTimeInterval = TimeSpan.FromSeconds(0.1);

    #endregion Private Fields

    #region Monitoring Methods
    public void ToggleAccelerometer()
    {
        if (Microsoft.Maui.Devices.Sensors.Accelerometer.Default.IsSupported)
        {
            if (!Microsoft.Maui.Devices.Sensors.Accelerometer.Default.IsMonitoring)
            {
                // Turn on accelerometer
                Microsoft.Maui.Devices.Sensors.Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                Microsoft.Maui.Devices.Sensors.Accelerometer.Default.Start(SensorSpeed.UI);
                AccelerometerViewModel.IsMonitoring = true;
            }
            else
            {
                // Turn off accelerometer
                Microsoft.Maui.Devices.Sensors.Accelerometer.Default.Stop();
                Microsoft.Maui.Devices.Sensors.Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                AccelerometerViewModel.IsMonitoring = false;
            }
        }
    }
    public void ToggleMagnetometer()
    {
        if (Microsoft.Maui.Devices.Sensors.Magnetometer.Default.IsSupported)
        {
            if (!Microsoft.Maui.Devices.Sensors.Magnetometer.Default.IsMonitoring)
            {
                // Turn on magnetometer
                Microsoft.Maui.Devices.Sensors.Magnetometer.Default.ReadingChanged += Magnetometer_ReadingChanged;
                Microsoft.Maui.Devices.Sensors.Magnetometer.Default.Start(SensorSpeed.UI);
                MagnetometerViewModel.IsMonitoring = true;
            }
            else
            {
                // Turn off magnetometer
                Microsoft.Maui.Devices.Sensors.Magnetometer.Default.Stop();
                Microsoft.Maui.Devices.Sensors.Magnetometer.Default.ReadingChanged -= Magnetometer_ReadingChanged;
                MagnetometerViewModel.IsMonitoring = false;
            }
        }
    }

    public void ToggleGyroscope()
    {
        if (Microsoft.Maui.Devices.Sensors.Gyroscope.Default.IsSupported)
        {
            if (!Microsoft.Maui.Devices.Sensors.Gyroscope.Default.IsMonitoring)
            {
                // Turn on gyroscope
                Microsoft.Maui.Devices.Sensors.Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
                Microsoft.Maui.Devices.Sensors.Gyroscope.Default.Start(SensorSpeed.UI);
                GyroscopeViewModel.IsMonitoring = true;
            }
            else
            {
                // Turn off gyroscope
                Microsoft.Maui.Devices.Sensors.Gyroscope.Default.Stop();
                Microsoft.Maui.Devices.Sensors.Gyroscope.Default.ReadingChanged -= Gyroscope_ReadingChanged;
                GyroscopeViewModel.IsMonitoring = false;
            }
        }
    }

    public void ToggleOrientation()
    {
        if (OrientationSensor.Default.IsSupported)
        {
            if (!OrientationSensor.Default.IsMonitoring)
            {
                // Turn on orientation
                OrientationSensor.Default.ReadingChanged += Orientation_ReadingChanged;
                OrientationSensor.Default.Start(SensorSpeed.UI);
            }
            else
            {
                // Turn off orientation
                OrientationSensor.Default.Stop();
                OrientationSensor.Default.ReadingChanged -= Orientation_ReadingChanged;
            }
        }
    }

    private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        // Update UI Label with accelerometer state
        AccelerometerViewModel.LatestData = new(DateTime.UtcNow, e.Reading.Acceleration * 9.81F);
    }
    private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
    {
        // Update UI Label with magnetometer state
        MagnetometerViewModel.LatestData = new(DateTime.UtcNow, e.Reading.MagneticField);
    }
    private void Orientation_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
    {
        // Update UI Label with orientation state
        var quaternion = e.Reading.Orientation;
        var eulerAngle = new Quaternion<double>(quaternion.W, quaternion.X, quaternion.Y, quaternion.Z).ToRotationMatrix().ToEulerAngles();
        //!手机轴系定义与库中定义不同
        eulerAngle = eulerAngle with { Yaw = Angle.RoundAngle - eulerAngle.Yaw };
        AttitudeDatas.Add((DateTime.UtcNow, eulerAngle));
    }
    private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
        // Update UI Label with gyroscope state
        GyroscopeViewModel.LatestData = new(DateTime.UtcNow, e.Reading.AngularVelocity);
        if (AttitudeDatas.Count == 0)
            return;
        var gyroLatestTime = GyroscopeViewModel.LatestData.DateTime;
        if (gyroLatestTime - AccelerometerViewModel.LatestData.DateTime > _maximumTimeInterval ||
            gyroLatestTime - MagnetometerViewModel.LatestData.DateTime > _maximumTimeInterval ||
            gyroLatestTime - AttitudeDatas[^1].UtcTime > _maximumTimeInterval)
            return;
        var accData = AccelerometerViewModel.LatestData.Value;
        var acceleration = accData.Length();
        var eulerAngle = AttitudeDatas[^1].EulerAngle;
        var gyroData = GyroscopeViewModel.LatestData.Value;
        var magData = MagnetometerViewModel.LatestData.Value;
        //var measurement = new DeadReckoningMeasurement(gyroLatestTime, accData, gyroData, magData);
        var measurement = new DeadReckoningMeasurement(gyroLatestTime, acceleration, eulerAngle);
        IntegratedDatas.Add((gyroLatestTime, accData, gyroData, magData, eulerAngle));
        IntegratedDatasChanged?.Invoke(this, new(measurement));
        //Debug.WriteLine($"Add IntegratedData:{gyroLatestTime:yyyy-MM-dd HH:mm:ss.fffffff} {accData} {magData} {eulerAngle}");
    }
    #endregion
}
