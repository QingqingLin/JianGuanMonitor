using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MonitorPorts
{
    class HashTable     //yao
    {
        #region diameter.xml-command
        public static Hashtable GetDiaHashtable()
        {
            Hashtable diaHash = new Hashtable();
            diaHash.Add(257, "Capabilities-Exchange");
            diaHash.Add(258, "Re-Auth");
            diaHash.Add(271, "Accounting");
            diaHash.Add(274, "Abort-Session");
            diaHash.Add(275, "Session-Termination");
            diaHash.Add(280, "Device-Watchdog");
            diaHash.Add(282, "Disconnect-Peer");
            diaHash.Add(310, "Boostrapping-Info");
            diaHash.Add(311, "Message-Process");
            diaHash.Add(312, "GBAPush-Info");
            diaHash.Add(314, "Policy-Data");
            diaHash.Add(315, "Policy-Install");
            diaHash.Add(316, "3GPP-Update-Location");
            diaHash.Add(317, "3GPP-Cancel-Location");
            diaHash.Add(318, "3GPP-Authentication-Information");
            diaHash.Add(319, "3GPP-Insert-Subscriber-Data");
            diaHash.Add(320, "3GPP-Delete-Subscriber-Data");
            diaHash.Add(321, "3GPP-Purge-UE");
            diaHash.Add(322, "3GPP-Reset");
            diaHash.Add(323, "3GPP-Notify");
            diaHash.Add(324, "3GPP-ME-Identity-Check");
            diaHash.Add(325, "MIP6");
            diaHash.Add(326, "QoS-Authorization");
            diaHash.Add(327, "QoS-Install");
            diaHash.Add(328, "Capabilities-Update");
            diaHash.Add(329, "IKEv2-SK");
            diaHash.Add(330, "NAT-Control");
            diaHash.Add(8388620, "3GPP-Provide-Location");
            diaHash.Add(8388621, "3GPP-Location-Report");
            diaHash.Add(8388622, "3GPP-LCS-Routing-Info");
            diaHash.Add(8388631, "Subscription Information Application");
            diaHash.Add(8388632, "Distributed Charging");
            diaHash.Add(8388633, "Ericsson-SL");
            diaHash.Add(8388634, "Ericsson-SN");
            diaHash.Add(8388635, "Spending-Limit");
            diaHash.Add(8388636, "Spending-Status-Notification");
            diaHash.Add(8388639, "3GPP-Device-Action");
            diaHash.Add(8388640, "3GPP-Device-Notification");
            diaHash.Add(8388641, "3GPP-Subscriber-Information");
            diaHash.Add(8388642, "Cancel-VCSG-Location");
            diaHash.Add(8388643, "3GPP-Device-Trigger");
            diaHash.Add(8388644, "3GPP-Delivery-Report");
            diaHash.Add(8388657, "Ericsson Binding-Data");
            diaHash.Add(8388717, "Ericsson Trace-Report");
            return diaHash;
        }
        #endregion

        #region diameter.xml-avp
        public static Hashtable GetAvpHashtable()
        {
            Hashtable avpHash = new Hashtable();
            avpHash.Add(263, "Session-Id");
            avpHash.Add(268, "Result-Code");
            avpHash.Add(1413, "Authentication-Info");
            avpHash.Add(1414, "E-UTRAN-Vector");
            avpHash.Add(1450, "KASME");
            return avpHash;
        }
        #endregion

        #region diameter.xml-result
        public static Hashtable GetResultHashtable()
        {
            Hashtable resultHash = new Hashtable();
            resultHash.Add(1001, "DIAMETER_MULTI_ROUND_AUTH--认证条件不满足");
            resultHash.Add(2001, "DIAMETER_SUCCESS--成功");
            resultHash.Add(2002, "DIAMETER_LIMITED_SUCCESS--部分完成，但提供给用户服务还需要附加过程");
            resultHash.Add(2003, "DIAMETER_FIRST_REGISTRATION--注册完成");
            resultHash.Add(2004, "DIAMETER_SUBSEQUENT_REGISTRATION随后的注册完成");
            resultHash.Add(2005, "DIAMETER_UNREGISTERED_SERVICE");
            resultHash.Add(2006, "DIAMETER_SUCCESS_SERVER_NAME_NOT_STORED");
            resultHash.Add(2007, "DIAMETER_SERVER_SELECTION");
            resultHash.Add(2008, "DIAMETER_SUCCESS_AUTH_SENT_SERVER_NOT_STORED");
            resultHash.Add(2009, "DIAMETER_SUCCESS_RELOCATE_HA");
            resultHash.Add(3001, "DIAMETER_COMMAND_UNSUPPORTED接受者无法识别请求的command code");
            resultHash.Add(3002, "DIAMETER_UNABLE_TO_DELIVER无法将信息传至目的地");
            resultHash.Add(3003, "DIAMETER_REALM_NOT_SERVED请求的目的范围无法被识别");
            resultHash.Add(3004, "DIAMETER_TOO_BUSY--对明确的服务器发出请求，但该服务器不能提供请求的服务");
            resultHash.Add(3005, "DIAMETER_LOOP_DETECTED由于配置错误传给的不是目标接受者");
            resultHash.Add(3006, "DIAMETER_REDIRECT_INDICATION--代理无法满足请求者的需求，会将能处理该强求的的服务器联系方式给请求者");
            resultHash.Add(3007, "DIAMETER_APPLICATION_UNSUPPORTED--有关该应用的请求不被支持");
            resultHash.Add(3008, "DIAMETER_INVALID_HDR_BITS--diameter消息透无效或command code与协议规定不一致");
            resultHash.Add(3009, "DIAMETER_INVALID_AVP_BITS--AVP Flag不被识别或与定义不符");
            resultHash.Add(3010, "DIAMETER_UNKNOWN_PEER--收到一个不被识别的用户发来的CER性能参数请求");
            resultHash.Add(4001, "DIAMETER_AUTHENTICATION_REJECTED--由于用户密码不对而导致认证不通过");
            resultHash.Add(4002, "DIAMETER_OUT_OF_SPACE--diameter节点由于缺乏空间无法计费");
            resultHash.Add(4003, "DIAMETER_ELECTION_LOST--由于没有参与竞争过程而掉线");
            resultHash.Add(4005, "DIAMETER_ERROR_MIP_REPLY_FAILURE");
            resultHash.Add(4006, "DIAMETER_ERROR_HA_NOT_AVAILABLE");
            resultHash.Add(4007, "DIAMETER_ERROR_BAD_KEY");
            resultHash.Add(4008, "DIAMETER_ERROR_MIP_FILTER_NOT_SUPPORTED");
            resultHash.Add(4010, "DIAMETER_END_USER_SERVICE_DENIED");
            resultHash.Add(4011, "DIAMETER_CREDIT_CONTROL_NOT_APPLICABLE");
            resultHash.Add(4012, "DIAMETER_CREDIT_LIMIT_REACHED");
            resultHash.Add(4013, "DIAMETER_USER_NAME_REQUIRED");
            resultHash.Add(5001, "DIAMETER_AVP_UNSUPPORTED--需强制被执行的AVP无法被识别");
            resultHash.Add(5002, "DIAMETER_UNKNOWN_SESSION_ID--被追踪用户的session-id无法被识别");
            resultHash.Add(5003, "DIAMETER_AUTHORIZATION_REJECTED--认证系统对该用户不开放");
            resultHash.Add(5004, "DIAMETER_INVALID_AVP_VALUE请求消息中包含无效的AVP");
            resultHash.Add(5005, "DIAMETER_MISSING_AVP--无法识别command code定义下指定的AVP");
            resultHash.Add(5006, "DIAMETER_RESOURCES_EXCEEDED--请求的资源超出服务器可以提供的");
            resultHash.Add(5007, "DIAMETER_CONTRADICTING_AVPS--服务器收到消息中的AVP互相矛盾");
            resultHash.Add(5008, "DIAMETER_AVP_NOT_ALLOWED--不被允许的AVP");
            resultHash.Add(5009, "DIAMETER_AVP_OCCURS_TOO_MANY_TIMES--AVP出现次数超过协议中所规定的");
            resultHash.Add(5010, "DIAMETER_NO_COMMON_APPLICATION--两个diameter节点无法共同应用");
            resultHash.Add(5011, "DIAMETER_UNSUPPORTED_VERSION--使用的协议版本不被支持");
            resultHash.Add(5012, "DIAMETER_UNABLE_TO_COMPLY--请求由于不明确的原因被拒绝");
            resultHash.Add(5013, "DIAMETER_INVALID_BIT_IN_HEADER--diameter消息投中出现了一个不被识别的位");
            resultHash.Add(5014, "DIAMETER_INVALID_AVP_LENGTH--AVP的长度是无效的");
            resultHash.Add(5015, "DIAMETER_INVALID_MESSAGE_LENGTH--消息长度是无效的");
            resultHash.Add(5016, "DIAMETER_INVALID_AVP_BIT_COMBO--AVP中包含AVP Flag中定义的不被允许的值");
            resultHash.Add(5017, "DIAMETER_NO_COMMON_SECURITY--两者之间没有共同的保障机制");
            resultHash.Add(5018, "DIAMETER_RADIUS_AVP_UNTRANSLATABLE");
            resultHash.Add(5024, "DIAMETER_ERROR_NO_FOREIGN_HA_SERVICE");
            resultHash.Add(5025, "DIAMETER_ERROR_END_TO_END_MIP_KEY_ENCRYPTION");
            resultHash.Add(5030, "DIAMETER_USER_UNKNOWN");
            resultHash.Add(5031, "DIAMETER_RATING_FAILED");
            resultHash.Add(5032, "DIAMETER_ERROR_USER_UNKNOWN");
            resultHash.Add(5033, "DIAMETER_ERROR_IDENTITIES_DONT_MATCH");
            resultHash.Add(5034, "DIAMETER_ERROR_IDENTITY_NOT_REGISTERED");
            resultHash.Add(5035, "DIAMETER_ERROR_ROAMING_NOT_ALLOWED");
            resultHash.Add(5036, "DIAMETER_ERROR_IDENTITY_ALREADY_REGISTERED");
            resultHash.Add(5037, "DIAMETER_ERROR_AUTH_SCHEME_NOT_SUPPORTED");
            resultHash.Add(5038, "DIAMETER_ERROR_IN_ASSIGNMENT_TYPE");
            resultHash.Add(5039, "DIAMETER_ERROR_TOO_MUCH_DATA");
            resultHash.Add(5040, "DIAMETER_ERROR_NOT SUPPORTED_USER_DATA");
            resultHash.Add(5041, "DIAMETER_ERROR_MIP6_AUTH_MODE");
            resultHash.Add(4241, "DIAMETER_END_USER_SERVICE_DENIED");
            resultHash.Add(5241, "DIAMETER_END_USER_NOT_FOUND");
            return resultHash;
        }
        #endregion

        //public Hashtable comHash = GetCommandHashtable();
        Hashtable avpHash = GetAvpHashtable();
        //public Hashtable resultHash = GetResultHashtable();
    }
}
