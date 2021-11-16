namespace ZitgaGiftCode
{
    public class LogicCode
    {
        public const int SUCCESS = 0;
        public const int FAIL = 1;
        public const int SERVER_ERROR = 2;
        public const int FEATURE_NOT_IMPLEMENTED = 3;
        public const int PLAYER_NOT_FOUND = 4;

        public const int INVALID_INPUT_DATA = 5;
        public const int NOT_ENOUGH_RESOURCES = 6;

        public const int INVALID_API_VERSION = 20;
        public const int INVALID_API_SECRET = 21;
        public const int INVALID_INBOUND_HASH = 22;

        public const int PERMISSION_NOT_GRANTED = 30;

        public const int GAME_NOT_FOUND = 40;
        public const int GAME_NAME_INVALID = 41;
        public const int GROUP_NAME_INVALID = 42;

        public const int GIFT_CODE_NOT_FOUND = 50;
        public const int GIFT_CODE_IS_ALREADY_USED = 51;
        public const int GIFT_CODE_IS_EXPIRED = 52;
        public const int ALREADY_CLAIMED_OTHER_FROM_SAME_GROUP = 53;

        public const int AUTH_TOKEN_INVALID = 60;

        public const int SORT_TAG_INVALID = 70;
        public const int PAGE_INVALID = 71;
        public const int SEARCH_QUERY_INVALID = 72;
        public const int SEARCH_TYPE_INVALID = 73;

        public const int GIFT_CODE_LENGTH_INVALID = 80;
        public const int GIFT_CODE_NUMBER_CREATE_INVALID = 81;
        public const int GIFT_CODE_GEN_TYPE_INVALID = 82;
        public const int GIFT_CODE_NUMBER_USAGE_INVALID = 83;
        public const int GIFT_CODE_PREFIX_INVALID = 84;
        public const int GIFT_CODE_INVALID = 85;
    }
}