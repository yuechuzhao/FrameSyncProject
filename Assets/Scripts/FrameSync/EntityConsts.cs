namespace Assets.Scripts.FrameSync {
    public static class EntityConsts {
        public static class EntityType {
            public static int FrameController = 1;
            public static int Player = 2;
            public static int InputController = 3;
            public static int SimulateLocalServer = 4;
        }

        public static class Message {
            public static string UNIT_CREATED = "UNIT_CREATED";
            public static string PLAY_OPERATION = "PLAY_OPERATION";
        }
    }
}