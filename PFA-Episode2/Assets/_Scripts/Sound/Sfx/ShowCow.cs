using System.Diagnostics;
using System.IO;
using UnityEngine;

public class ShowCow : MonoBehaviour
{
    public static void ShowCowInCmd(string message, int time)
    {
        string cow = $@"
                                       /;    ;\
                                   __  \\____//
                                  /{{_\_/   `'\____
                                  \___   (o)  (o  }}
       _____________________________/          :--'  
   ,-,'`@@@@@@@@       @@@@@@         \_    `__\       -----------
  ;:(  @@@@@@@@@        @@@             \___(o'o)   <  {message} >
  :: )  @@@@          @@@@@@        ,'@@(  `===='      -----------
  :: : @@@@@:          @@@@         `@@@:
  :: \  @@@@@:       @@@@@@@)    (  '@@@'
  ;; /\      /`,    @@@@@@@@@\   :@@@@@)
  ::/  )    {{_----------------:  :~`,~~;
 ;;'`; :   )                  :  / `; ;
;;;; : :   ;                  :  ;  ; :              
`'`' / :  :                   :  :  : :
    )_ \__;                   :_ ;  \_\    
    :__\  \                    \  \  :  \  
        `^'                     `^'  `-^-'  
";

        // Chemin vers un fichier temporaire
        string tempPath = Path.Combine(Application.dataPath, "cow.txt");
        File.WriteAllText(tempPath, cow);

        // Prépare la commande : afficher le fichier, attendre 2 secondes, quitter
        string cmdArgs = $"/C type \"{tempPath}\" & timeout /T {time} /NOBREAK";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = cmdArgs,
            UseShellExecute = true,
        };

        try
        {
            Process.Start(psi);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Erreur : " + e.Message);
        }
    }
}