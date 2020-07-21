using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject playerName;
    public Transform playersScrollView;
    public GameObject knight;
    public GameObject paladin;
    public string[] members;
    public List<Entity> players = new List<Entity>();
    public SpawnArea spawnArea;
    public GameObject[] bloodOnGround;
    public GameObject[] bloodOnPlayer;
    public Text warriorsText;
    int warriorsCount = 0;
    int deathsCount = 0;

    private void Start() 
    {
        members = new string[] {
           "JokerGamer#9447","Torvi#7460","jhon wick#2294","MuriloBona#8915","MrDurand#9923","Maycaco_157#1788","XuXu Play#5799","pecegondo#4663","Arrebatador_Max#1091","Morzan#9527","Sloth#5047","Daniel Heringer#6103","Leandro#4314","vitorFelipe#3426","Guru †Mu Mobile ON†#7952","Narix_12#7643","k0guMello#3619","juninho#2685","Brendon#0477","Full_Js#1123","IgorD#3679","Uenderli#8233","KrioK#7561","DougBaster#5716","SouMuitoLoko#8794","Fan#6777","Madrugadão#8619","Donate Bot#0710","Rick#9557","JonasPKQW#0651","Kaminoraion#9743","caosgab#3962","Bob Marley#2147","aGARAgun#3704","Ona9#2587","IDGAF (Guéfi)#9043","GabrielSaut13#5022","PHH#1833","Jaelking#4064","wellfbrito#0569","outsiders_darks#2593","cidralx#6335","tiago#1538","brenoca#1213","icDeath#9390","Dino DC#7873","Juliana Medeiros#6273","BeithFerk#0814","Ryrytoxd#2763","Vallkyriel twitch.tv/vallkyriel#9911","sektorKill#9072","Lous#6093","NWE BrukaYT#1314","Hizzune#0038","S4S5_98#1162","ZenXBR#7134","TRINDADE#5797","DarkHawk#8915","Jnobre#3311","Jhow#3736","Niih Santana#8288","RAFAEL#6917","FELIPE#9664","Litwick#9414","yFloreDz#7030","Eminiales#3874","developer musniper#2796","Seduruk#3743","Tio Paçoca#4594","nemotz#3769","Renan Lopes#3139","Willyan#1896","chubba_TV#9455","AlexZ3ROcinco#5842","Denky#7900","Pedrin17#2581","Gado filosofo#3168","hugo#2371","mandi#4501","Ggibamede#7041","Niih Santana#3298","MEE6#4876","JaySee#6091","Warasuo#2717","SiiN_#7040","Ricardo Bruno#5445","MinhaPrimeiraVez#1860","All Might#9668","LemoKing#2742","GaaZ#1388","TwoTVBot#0116","twitch.tv/FNGSilva#4953","WillNavs#1098","Wynott_#9391","LanTerNados#8964","Léo Silva#4372","Luisph#0126","Schizo#8837","Mark Wolf#2582","Lust#9710","Zirigidun#2445","henriqueshb#8033","Taiacu#5378","Alessandro#5094","ViniJacob#8164","Alexsander Cesar#4874","DomGiovanni#8755","GallighanMaker#6663","Um Desocupado#9399","Two. Tv#2522","Yuuya#0515","Vinicius Mota#7670","Kidchibata#7473","Framk#3143","_KeticiaRaf#9458","Wyll#7644","Jr_Amorimm#4528","icDeath#7014","Jusezin Uchirra#7512","Doug Dogtown#0236","Emerson#6563","drop#9260","TaubatexasPlay#2702","Vini#9956","RyanCruz#4298","HAVEKAR#6681","SKULL#0798","Hugo05#1352","DarkFire#67","Flash#6810"
        };

        CreatePlayers();
        SpawnPlayers();

        StartCoroutine(ScaleOverTime(240));

        warriorsCount = members.Length;
        warriorsText.text = String.Format("Warriors | Total: {0} | Dead: {1} | Fighting: {2}", warriorsCount, deathsCount, warriorsCount - deathsCount);
    }

    void CreatePlayers()
    {
        foreach(string member in members)
        {
            Entity e = new Entity();
            e = GenerateRandomAttributes(e);
            e = PickRandomClass(e);
            e.entityName = member;
            e.kills = 0;
            players.Add(e);
        }
    }

    Entity PickRandomClass(Entity e)
    {
        int classId = UnityEngine.Random.Range(0, 2);

        if(classId == 0)
        {
            e.entityClass = EntityClass.Paladin; 
            e.prefab = paladin;
        }
        else if(classId == 1)
        {
            e.entityClass = EntityClass.Knight;
            e.prefab = knight;
        }  

        return e;
    }

    Entity GenerateRandomAttributes(Entity e)
    {
        e.speed = UnityEngine.Random.Range(2, 6);
        e.damage = UnityEngine.Random.Range(4, 10);
        e.defense = UnityEngine.Random.Range(4, 10);
        e.weaponDamage = UnityEngine.Random.Range(2, 10);
        e.armorDefense = UnityEngine.Random.Range(2, 10);
        return e;
    }

    void SpawnPlayers()
    {
        foreach(Entity e in players)
        {
            var randomPos = RandomPointInBounds(spawnArea.boxCollider.GetComponent<Collider>().bounds);

            GameObject player = Instantiate(e.prefab, randomPos, transform.rotation);
            player.GetComponent<Player>().entity = e;
            
            GameObject pName = Instantiate(playerName, playersScrollView);
            pName.name = e.entityName;
            pName.GetComponent<Text>().text = String.Format("{0} | Kills: {1}", e.entityName, e.kills);
            pName.transform.SetParent(playersScrollView);
        }
    }

    public Vector3 RandomPointInBounds(Bounds bounds) 
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void UpdateKills(Entity e)
    {
         foreach (Transform child in playersScrollView)
        {
            if (child.name == e.entityName)
            {        
                e.kills ++;         
                child.GetComponent<Text>().text = String.Format("{0} | Kills: {1}", e.entityName, e.kills);              
            }
        }    
    }

    public void RemovePlayerFromList(Entity e)
    {
        foreach (Transform child in playersScrollView)
        {
            if (child.name == e.entityName)
            {
                deathsCount++;
                child.GetComponent<Text>().color = Color.red;
                warriorsText.text = String.Format("Warriors | Total: {0} | Dead: {1} | Fighting: {2}", warriorsCount, deathsCount, warriorsCount - deathsCount);
            }
        }
    }

    IEnumerator ScaleOverTime(float time)
    {
        Vector3 originalScale = spawnArea.boxCollider.size;
        Vector3 destinationScale = new Vector3(0.3f, 1f, 0.3f);

        float currentTime = 0.0f;

        do
        {
            Vector3 scale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            spawnArea.boxCollider.size = scale;
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }
}
