namespace Assets.Scripts.FrameSync {
    public static class EntityConsts {
        public static class EntityType {
            public static int FrameController = 1;
            public static int Unit = 2;
            public static int InputController = 3;
            public static int SimulateLocalServer = 4;
            public static int UnitManager = 5;
        }

        public static class Message {
            public static string JOIN_GAME = "JOIN_GAME";
            public static string PLAY_OPERATION = "PLAY_OPERATION";
            public static string ACTIVATE_SELF = "ACTIVATE_SELF";
            public static string CREATE_UNIT = "CREATE_UNIT";
            public static string UNIT_MOVE = "UNIT_MOVE";
        }
    }
}