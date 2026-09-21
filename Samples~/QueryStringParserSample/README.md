# How to use the sample

La scena QuerystringParserSample contiene tutto ciò che serve per parsare i dati che arrivano in querystring dall'url del browser,
e parsarli nei dati che servono per collegare un'app esterna all'ecosistema di Virtuademy.

L'oggetto GameManager contiene gli script principali per gestire il flow.

BrowserUrlParser è lo script specifico per WebGL che si occupa di leggere l'url e parsare i parametri in querystring.
Esso fornisce il metodo ParseUrlParameters(), che restituisce un dizionario <string, string> che sono le chiavi e i valori della querystring.

QuerystringParserSample è lo script specifico di questo esempio, che si occupa di tradurre il dizionario ottenuto da BrowserUrlParser
e utilizzare i dati contenuti in esso. Può essere usato anche in altre applicazioni esterne senza apportare modifiche.
Che parametri servono all'applicazione esterna per funzionare correttamente dentro Virtuademy?

- authSessionHash: è l'hash di sessione che serve per identificare l'utente
  e fare le chiamate HMAC della profile api per ottenere i token delle altre api.
- worldId: id del mondo, serve perchè le metriche di esperienza e presenza sono indicizzate per mondo.
- experienceId: l'applicazione esterna deve creare una sessione single player associata all'esperienza
  che viene passata da Virtuademy, successivamente connettersi al websocket.
  La combinazione di id del mondo e sessione permette di fare le chiamate alle api delle analitiche.

Per salvare un'analitica di esperienza, è necessario chiamare l'endpoint della Application API `CreateExperienceAnalytic(ExperienceAnalyticDTO analytic)`

experienceAnalytics è un oggetto di tipo `ExperienceAnalyticDTO` strutturato come segue:

```
public class ExperienceAnalyticDTO
{
    EAnalyticVerb verb;
    int sessionId; //Deve essere valorizzato con la sessione recuparata in precedenza
    object customAttributes;
    XAPIStatement statement; // Deve essere settato a null, altrimenti l'analitica viene mandata alle XAPI
    string locale; // Serve solo per XAPI, si può ignorare
    string context;
}
```

Tuttavia, questo oggetto è astratto, quindi a livello pratico andrà creato uno dei suoi oggetti derivati:
ExperienceJoinDTO, ExperienceStartDTO, ExperienceCompleteDTO, ExperienceStepStartDTO, ExperienceStepCompleteDTO, ExperienceTranscriptDTO
