using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.TextToSpeech.v1;
using NAudio.Wave;

namespace WatsonTextSpeech
{
  class Program
  {
    async static Task Main(string[] args)
    {
      await GetAudio();

      using (var waveOut = new WaveOutEvent())
      using (var wavReader = new WaveFileReader(@"./newsletter.wav"))
      {
        waveOut.Init(wavReader);
        waveOut.Play();

        while (waveOut.PlaybackState == PlaybackState.Playing)
        {
          Thread.Sleep(1000);
        }
      }
    }

    public static string FormatMessage(string message) => $"Olá meu xuxuzinho!, sou eu a Isa. As noticias de hoje são sobre Malware grave no Android, CPU neuromórfica da Intel, e também sobre Brasil é destaque em GovTech. Vamos lá? {message}";

    public async static Task GetAudio() {
      var authenticator = new IamAuthenticator(
        apikey: "lFDwDxrcLwZi0FPP3qjwBEWpBzH_7y8IWM9tg9L1Jdl4"
      );

      var textToSpeech = new TextToSpeechService(authenticator);

      textToSpeech.SetServiceUrl("https://api.us-east.text-to-speech.watson.cloud.ibm.com/instances/86fad6c7-1bd6-4e1c-9f6f-1f06fe38d358");
      textToSpeech.DisableSslVerification(true);
      
      var result = textToSpeech.Synthesize(
        text: FormatMessage(@"Novo malware é descoberto no Android: estima-se que o GriftHorse já tenha infectado mais de 10 milhões de dispositivos globalmente e estava presente em diversos aplicativos na Play Store (o Google já os removeu, mas ainda podem estar circulando em outras lojas). O trojan engana os usuários pedindo seus números de telefone para ganharem um prêmio, mas ao invés disso, acaba confirmando um serviço de assinatura baseado em SMS. A lista com mais de uma centena de apps maliciosos pode ser conferida no blog da empresa de segurança Zimperium e recomenda-se que sejam desinstalados imediatamente.  "),
        accept: "audio/wav",
        voice: "pt-BR_IsabelaV3Voice"
      );

      await using (FileStream fs = File.Create("./newsletter.wav"))
      {
        result.Result.WriteTo(fs);
        fs.Close();
        result.Result.Close();
      }
    }
  }
}
