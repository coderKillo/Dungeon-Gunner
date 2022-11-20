using UnityEngine;

public static class Animations
{
    #region PLAYER
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int dashUp = Animator.StringToHash("dashUp");
    public static int dashDown = Animator.StringToHash("dashDown");
    public static int dashLeft = Animator.StringToHash("dashLeft");
    public static int dashRight = Animator.StringToHash("dashRight");

    public static float playerAnimationBaseSpeed = 8f;
    public static float enemyAnimationBaseSpeed = 3f;
    #endregion

    #region DOOR
    public static int open = Animator.StringToHash("open");
    #endregion

    #region TABLE
    public static int flipRight = Animator.StringToHash("flipRight");
    public static int flipLeft = Animator.StringToHash("flipLeft");
    public static int flipDown = Animator.StringToHash("flipDown");
    public static int flipUp = Animator.StringToHash("flipUp");
    #endregion

    #region DESTROY
    public static int destroy = Animator.StringToHash("destroy");
    public static string stateDestroyed = "Destroyed";
    #endregion

    #region CHEST
    public static int openChest = Animator.StringToHash("use");
    #endregion
}
