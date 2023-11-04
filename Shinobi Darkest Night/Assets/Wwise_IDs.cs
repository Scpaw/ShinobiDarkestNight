namespace AK
{
    public class EVENTS
    {
        public static uint DESUMIRU_ATTACK_ELEKTRYCZNYKUSARIGAMY = 458242830U;
        public static uint DESUMIRU_ATTACK_KRECENIEKUSARIGAMY = 2717276446U;
        public static uint DESUMIRU_ATTACK_OBROTKUSARIGAMY = 1250704626U;
        public static uint ENEMY_ARCHER_DAMAGE = 2902518741U;
        public static uint ENEMY_ARCHER_DEATH = 3402709392U;
        public static uint ENEMY_ARCHER_MOVINGLP = 3245943036U;
        public static uint ENEMY_ARCHER_PULL = 686651427U;
        public static uint ENEMY_ARCHER_RELEASE = 2805720177U;
        public static uint ENEMY_BULL_ATTACK = 3212818328U;
        public static uint ENEMY_BULL_DAMAGE = 2110191999U;
        public static uint ENEMY_BULL_DEATH = 4139684570U;
        public static uint ENEMY_BULL_MOVINGLP = 1144400610U;
        public static uint ENEMY_MINIOKUBI_DAMAGE = 330398815U;
        public static uint ENEMY_MINIOKUBI_DASHINGLP = 1761771660U;
        public static uint ENEMY_MINIOKUBI_DEATH = 2383443770U;
        public static uint ENEMY_MINIOKUBI_MOVINGLP = 1924261826U;
        public static uint ENEMY_RONIN_ATTACK = 4057789033U;
        public static uint ENEMY_RONIN_DAMAGE = 3731808246U;
        public static uint ENEMY_RONIN_DEATH = 802678365U;
        public static uint ENEMY_RONIN_MOVINGLP = 1078365811U;
        public static uint ENEMY_SCARECROW_ATTACK = 2599359430U;
        public static uint ENEMY_SCARECROW_DAMAGE = 1839043697U;
        public static uint ENEMY_SCARECROW_DEATH = 3676569724U;
        public static uint ENEMY_SCARECROW_STANDINGLP = 2917139710U;
        public static uint ENEMY_SPEARGUY_ATTACK = 783605241U;
        public static uint ENEMY_SPEARGUY_DAMAGE = 667626246U;
        public static uint ENEMY_SPEARGUY_DEATH = 1059221869U;
        public static uint ENEMY_SPEARGUY_MOVINGLP = 3956532163U;
        public static uint ENEMY_WLOCZNIK_ATTACK = 1443145844U;
        public static uint ENEMY_WLOCZNIK_DAMAGE = 1842470059U;
        public static uint ENEMY_WLOCZNIK_DEATH = 3328007190U;
        public static uint ENEMY_WLOCZNIK_MOVINGLP = 4235526766U;
        public static uint ITAIKEN_BLOOD_ATTACK = 162003230U;
        public static uint ITAIKEN_BLOODSWORD_ATTACK = 3161759601U;
        public static uint PLAY_STOP_TESTBEEP_LP = 2963813288U;
        public static uint PLAY_STOP_TESTBEEP_LP_3D = 1619838726U;
        public static uint PLAY_TESTBEEP = 1955951874U;
        public static uint PLAY_TESTBEEP_3D = 1681067324U;
        public static uint PLAY_TESTBEEP_LP = 858964037U;
        public static uint PLAY_TESTBEEP_LP_3D = 161393517U;
        public static uint PLAYER_DAMAGE = 2074073782U;
        public static uint PLAYER_DASH = 2394582229U;
        public static uint PLAYER_DEATH = 3083087645U;
        public static uint PLAYER_EATING_SLODYCZ = 384837272U;
        public static uint PLAYER_HEALING_DRINKING = 1926537714U;
        public static uint PLAYER_HEALING_ZOOM = 3307141145U;
        public static uint PLAYER_SLASHING = 1893567068U;
        public static uint SHOKYAKU_FIRE_ATTACK_LP = 2444959333U;
        public static uint SHOKYAKU_FIRE_ATTACK_STOP = 984524449U;
        public static uint SHURIKEN_GRABBING = 1899396691U;
        public static uint SHURIKEN_THROWING = 3416602747U;
        public static uint STOP_ENEMY_ARCHER_MOVING = 1810806391U;
        public static uint STOP_ENEMY_BULL_MOVING = 1267737121U;
        public static uint STOP_ENEMY_MINIOKUBI_DASHING = 965640407U;
        public static uint STOP_ENEMY_MINIOKUBI_MOVING = 1027764427U;
        public static uint STOP_ENEMY_SCARECROW_STANDING = 959072311U;
        public static uint STOP_ENEMY_SPEARGUY_MOVING = 3349970732U;
        public static uint STOP_ENEMY_WLOCZNIK_MOVING = 3909859073U;
        public static uint UI_CLICK = 2249769530U;
        public static uint UI_POINT = 4228838608U;
    } // public class EVENTS

    public class STATES
    {
        public class AREA_STATE
        {
            public static uint GROUP = 3979687176U;

            public class STATE
            {
                public static uint NONE = 748895195U;
                public static uint OUTDOORS = 2730119150U;
            } // public class STATE
        } // public class AREA_STATE

        public class GAMESTATUS
        {
            public static uint GROUP = 1045871717U;

            public class STATE
            {
                public static uint INGAME = 984691642U;
                public static uint INMENU = 3374585465U;
                public static uint NONE = 748895195U;
            } // public class STATE
        } // public class GAMESTATUS

        public class PLAYER_STATE
        {
            public static uint GROUP = 4071417932U;

            public class STATE
            {
                public static uint ALIVE = 655265632U;
                public static uint DEAD = 2044049779U;
                public static uint NONE = 748895195U;
            } // public class STATE
        } // public class PLAYER_STATE

    } // public class STATES

    public class SWITCHES
    {
        public class PLAYERHEALTH
        {
            public static uint GROUP = 151362964U;

            public class SWITCH
            {
                public static uint FULLHEALTH = 2429688720U;
                public static uint LOWHEALTH = 1017222595U;
                public static uint NEARDEATH = 898449699U;
            } // public class SWITCH
        } // public class PLAYERHEALTH

        public class PLAYERSPEEDSWITCH
        {
            public static uint GROUP = 2051106367U;

            public class SWITCH
            {
                public static uint RUN = 712161704U;
                public static uint WALK = 2108779966U;
            } // public class SWITCH
        } // public class PLAYERSPEEDSWITCH

    } // public class SWITCHES

    public class GAME_PARAMETERS
    {
        public static uint RTPC_DISTANCE = 262290038U;
        public static uint RTPC_PLAYERSPEED = 2653406601U;
    } // public class GAME_PARAMETERS

    public class BANKS
    {
        public static uint INIT = 1355168291U;
        public static uint MAIN = 3161908922U;
    } // public class BANKS

    public class BUSSES
    {
        public static uint _2DAMBIENCE = 309309195U;
        public static uint _2DAMBIENTBEDS = 4152869693U;
        public static uint _3DAMBIENCE = 1301074112U;
        public static uint AMBIENTBEDS = 1182634443U;
        public static uint AMBIENTMASTER = 1459460693U;
        public static uint MASTER_AUDIO_BUS = 3803692087U;
        public static uint NPCMASTER = 2033911932U;
        public static uint PLAYERATTACK = 2169369406U;
        public static uint PLAYERCLOTH = 765206498U;
        public static uint PLAYERLOCOMOTION = 2343802269U;
        public static uint PLAYERMASTER = 3538689948U;
    } // public class BUSSES

    public class AUX_BUSSES
    {
        public static uint CAVE = 4122393694U;
        public static uint OUTDOOR = 144697359U;
        public static uint REVERBS = 3545700988U;
    } // public class AUX_BUSSES

    public class AUDIO_DEVICES
    {
        public static uint NO_OUTPUT = 2317455096U;
        public static uint SYSTEM = 3859886410U;
    } // public class AUDIO_DEVICES

}// namespace AK

