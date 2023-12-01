using NAudio.Wave;
using System;
using System.IO;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private WasapiLoopbackCapture capture;
    private WaveFileWriter writer;
    private string outputPath;

    public MainForm()
    {
        InitializeComponent();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Wave File (*.wav)|*.wav";
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            outputPath = saveFileDialog.FileName;

            capture = new WasapiLoopbackCapture();
            capture.DataAvailable += Capture_DataAvailable;
            capture.RecordingStopped += Capture_RecordingStopped;

            writer = new WaveFileWriter(outputPath, capture.WaveFormat);

            capture.StartRecording();
            Task.Delay(30000).ContinueWith(t => StopRecording());
        }
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        StopRecording();
    }

    private void Capture_DataAvailable(object sender, WaveInEventArgs e)
    {
        writer.Write(e.Buffer, 0, e.BytesRecorded);
    }

    private void Capture_RecordingStopped(object sender, StoppedEventArgs e)
    {
        writer?.Dispose();
        writer = null;
        capture.Dispose();
    }

    private void StopRecording()
    {
        if (capture != null)
        {
            capture.StopRecording();
        }
    }
}
