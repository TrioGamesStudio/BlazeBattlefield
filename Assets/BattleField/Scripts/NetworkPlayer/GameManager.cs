public static class GameManager
{
    public static string[] names = new string[] {
        "John",
        "Alice",
        "Michael",
        "Emma",
        "David",
        "Sophia",
        "Rabbit",
        "Cat",
        "Tiger",
        "Monkey"
    };

    // mainmenu input file nickName gan vao day -> khi spawn this.nickName gan cho RPC_SetNickName() coll 63 NetworkPlayer
    public static string playerNickName = null;
    public static bool isEnemy;
}