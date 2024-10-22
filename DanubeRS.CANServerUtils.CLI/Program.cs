// See https://aka.ms/new-console-template for more information

using DanubeRS.CanServerUtils.Lib.BinaryLogs;
using DanubeRS.CanServerUtils.Lib.DBC;
using DanubeRS.CanServerUtils.Lib.DBC.Parser;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(b => { b.AddConsole(); });

var dbc = new Database();
var testString = @"
VERSION """"


NS_ : 
	NS_DESC_
	CM_
	BA_DEF_
	BA_
	VAL_
	CAT_DEF_
	CAT_
	FILTER
	BA_DEF_DEF_
	EV_DATA_
	ENVVAR_DATA_
	SGTYPE_
	SGTYPE_VAL_
	BA_DEF_SGTYPE_
	BA_SGTYPE_
	SIG_TYPE_REF_
	VAL_TABLE_
	SIG_GROUP_
	SIG_VALTYPE_
	SIGTYPE_VALTYPE_
	BO_TX_BU_
	BA_DEF_REL_
	BA_REL_
	BA_DEF_DEF_REL_
	BU_SG_REL_
	BU_EV_REL_
	BU_BO_REL_
	SG_MUL_VAL_

BS_:

BU_: Receiver ChassisBus VehicleBus PartyBus


BO_ 12 ID00CUI_status: 8 VehicleBus
 SG_ UI_audioActive : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autopilotTrial : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_bluetoothActive : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cameraActive : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cellActive : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cellConnected : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cellNetworkTechnology : 19|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_cellReceiverPower : 24|8@1+ (1,-128) [-128|127] ""dB""  Receiver
 SG_ UI_cellSignalBars : 42|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_cpuTemperature : 56|8@1- (1,40) [-20|100] ""C""  Receiver
 SG_ UI_developmentCar : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_displayOn : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_displayReady : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_factoryReset : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_falseTouchCounter : 32|8@1+ (1,0) [0|255] ""1""  Receiver
 SG_ UI_gpsActive : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_pcbTemperature : 48|8@1- (1,40) [-20|100] ""C""  Receiver
 SG_ UI_radioActive : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_readyForDrive : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_screenshotActive : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_systemActive : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_touchActive : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_vpnActive : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_wifiActive : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_wifiConnected : 7|1@1+ (1,0) [0|1] """"  Receiver

BO_ 851 ID353UI_status: 8 VehicleBus
 SG_ UI_audioActive : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autopilotTrial : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_bluetoothActive : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cameraActive : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cellActive : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cellConnected : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cellNetworkTechnology : 19|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_cellReceiverPower : 24|8@1+ (1,-128) [-128|127] ""dB""  Receiver
 SG_ UI_cellSignalBars : 42|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_cpuTemperature : 56|8@1- (1,40) [-20|100] ""C""  Receiver
 SG_ UI_developmentCar : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_displayOn : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_displayReady : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_factoryReset : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_falseTouchCounter : 32|8@1+ (1,0) [0|255] ""1""  Receiver
 SG_ UI_gpsActive : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_pcbTemperature : 48|8@1- (1,40) [-20|100] ""C""  Receiver
 SG_ UI_radioActive : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_readyForDrive : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_screenshotActive : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_systemActive : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_touchActive : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_vpnActive : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_wifiActive : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_wifiConnected : 7|1@1+ (1,0) [0|1] """"  Receiver

BO_ 22 ID016DI_bmsRequest: 1 VehicleBus
 SG_ DI_bmsOpenContactorsRequest : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_bmsRequestInterfaceVersion : 0|4@1+ (1,0) [0|15] """"  Receiver

BO_ 130 ID082UI_tripPlanning: 8 VehicleBus
 SG_ UI_energyAtDestination : 48|16@1- (0.01,0) [-327.67|327.67] ""kWh""  Receiver
 SG_ UI_hindsightEnergy : 32|16@1- (0.01,0) [-327.67|327.67] ""kWh""  Receiver
 SG_ UI_navToSupercharger : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_predictedEnergy : 16|16@1- (0.01,0) [-327.67|327.67] ""kWh""  Receiver
 SG_ UI_requestActiveBatteryHeating : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_tripPlanningActive : 0|1@1+ (1,0) [0|1] """"  Receiver

BO_ 257 ID101RCM_inertial1: 8 ChassisBus
 SG_ RCM_inertial1Checksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ RCM_inertial1Counter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ RCM_pitchRate : 16|15@1- (0.00025,0) [-4.096|4.09575] ""rad/s""  Receiver
 SG_ RCM_pitchRateQF : 50|2@1+ (1,0) [0|3] """"  Receiver
 SG_ RCM_rollRate : 31|15@1- (0.00025,0) [-4.096|4.09575] ""rad/s""  Receiver
 SG_ RCM_rollRateQF : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ RCM_yawRate : 0|16@1- (0.0001,0) [-3.2766|3.2766] ""rad/s""  Receiver
 SG_ RCM_yawRateQF : 48|1@1+ (1,0) [0|1] """"  Receiver

BO_ 273 ID111RCM_inertial2: 8 ChassisBus
 SG_ RCM_inertial2Checksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ RCM_inertial2Counter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ RCM_lateralAccel : 16|16@1- (0.00125,0) [-40.9575|40.9575] ""m/s^2""  Receiver
 SG_ RCM_lateralAccelQF : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ RCM_longitudinalAccel : 0|16@1- (0.00125,0) [-40.9575|40.9575] ""m/s^2""  Receiver
 SG_ RCM_longitudinalAccelQF : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ RCM_verticalAccel : 32|16@1- (0.00125,0) [-40.9575|40.9575] ""m/s^2""  Receiver
 SG_ RCM_verticalAccelQF : 50|1@1+ (1,0) [0|1] """"  Receiver

BO_ 278 RCM_inertial2New: 8 ChassisBus
 SG_ RCM_inertial2Checksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ RCM_inertial2Counter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ RCM_lateralAccel : 16|16@1- (0.00125,0) [-40.9575|40.9575] ""m/s^2""  Receiver
 SG_ RCM_lateralAccelQF : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ RCM_longitudinalAccel : 0|16@1- (0.00125,0) [-40.9575|40.9575] ""m/s^2""  Receiver
 SG_ RCM_longitudinalAccelQF : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ RCM_verticalAccel : 32|16@1- (0.00125,0) [-40.9575|40.9575] ""m/s^2""  Receiver
 SG_ RCM_verticalAccelQF : 50|1@1+ (1,0) [0|1] """"  Receiver

BO_ 258 ID102VCLEFT_doorStatus: 8 VehicleBus
 SG_ VCLEFT_frontHandlePWM : 16|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCLEFT_frontHandlePulled : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_frontHandlePulledPersist : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_frontIntSwitchPressed : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_frontLatchStatus : 0|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCLEFT_frontLatchSwitch : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_frontRelActuatorSwitch : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_mirrorDipped : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_mirrorFoldState : 52|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_mirrorHeatState : 58|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_mirrorRecallState : 55|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCLEFT_mirrorState : 49|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_mirrorTiltXPosition : 33|8@1+ (0.02,0) [0|5] ""V""  Receiver
 SG_ VCLEFT_mirrorTiltYPosition : 41|8@1+ (0.02,0) [0|5] ""V""  Receiver
 SG_ VCLEFT_rearHandlePWM : 24|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCLEFT_rearHandlePulled : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_rearIntSwitchPressed : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_rearLatchStatus : 4|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCLEFT_rearLatchSwitch : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_rearRelActuatorSwitch : 13|1@1+ (1,0) [0|1] """"  Receiver

BO_ 259 ID103VCRIGHT_doorStatus: 8 VehicleBus
 SG_ VCRIGHT_frontHandlePWM : 14|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCRIGHT_frontHandlePulled : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_frontHandlePulledPersist : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_frontIntSwitchPressed : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_frontLatchStatus : 0|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCRIGHT_frontLatchSwitch : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_frontRelActuatorSwitch : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_mirrorDipped : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_mirrorFoldState : 52|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCRIGHT_mirrorRecallState : 60|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCRIGHT_mirrorState : 49|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCRIGHT_mirrorTiltXPosition : 33|8@1+ (0.02,0) [0|5] ""V""  Receiver
 SG_ VCRIGHT_mirrorTiltYPosition : 41|8@1+ (0.02,0) [0|5] ""V""  Receiver
 SG_ VCRIGHT_rearHandlePWM : 21|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCRIGHT_rearHandlePulled : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_rearIntSwitchPressed : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_rearLatchStatus : 4|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCRIGHT_rearLatchSwitch : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_rearRelActuatorSwitch : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_reservedForBackCompat : 28|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_trunkLatchStatus : 56|4@1+ (1,0) [0|8] """"  Receiver

BO_ 275 ID113GTW_bmpDebug: 3 VehicleBus
 SG_ GTW_BMP_AWAKE_PIN : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_GTW_PMIC_ERROR : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_GTW_PMIC_ON : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_GTW_PMIC_THERMTRIP : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_GTW_SOC_PWROK : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_GTW_nSUSPWR : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_PERIPH_nRST_3V3_PIN : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_PGOOD_PIN : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_BMP_PMIC_PWR_ON : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_bmpState : 7|8@0+ (1,0) [0|255] """"  Receiver

BO_ 281 ID119VCSEC_windowRequests: 2 VehicleBus
 SG_ VCSEC_hvacRunScreenProtectOnly : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCSEC_windowRequestLF : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCSEC_windowRequestLR : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCSEC_windowRequestPercent : 8|7@1+ (1,0) [0|127] """"  Receiver
 SG_ VCSEC_windowRequestRF : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCSEC_windowRequestRR : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCSEC_windowRequestType : 4|2@1+ (1,0) [0|3] """"  Receiver

BO_ 290 ID122VCLEFT_doorStatus2: 6 VehicleBus
 SG_ VCLEFT_frontDoorState : 17|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_frontHandle5vEnable : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_frontHandleDebounceStatus : 32|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCLEFT_frontHandleRawStatus : 24|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCLEFT_frontLatchRelDuty : 0|8@1+ (1,0) [0|255] ""%""  Receiver
 SG_ VCLEFT_mirrorFoldMaxCurrent : 40|7@1- (0.046,0) [-2.944|2.898] ""A""  Receiver
 SG_ VCLEFT_rearDoorState : 20|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_rearHandle5vEnable : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_rearHandleDebounceStatus : 35|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCLEFT_rearHandleRawStatus : 27|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCLEFT_rearLatchRelDuty : 8|8@1+ (1,0) [0|255] ""%""  Receiver
 SG_ VCLEFT_vehicleInMotion : 16|1@1+ (1,0) [0|1] """"  Receiver

BO_ 291 ID123UI_alertMatrix1: 8 VehicleBus
 SG_ UI_a001_DriverDoorOpen : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a002_DoorOpen : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a003_TrunkOpen : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a004_FrunkOpen : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a005_HeadlightsOnDoorOpen : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a006_RemoteServiceAlert : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a007_SoftPackConfigMismatch : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a008_TouchScreenError : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a009_SquashfsError : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a010_MapsMissing : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a011_IncorrectMap : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a012_NotOnPrivateProperty : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a013_TPMSHardWarning : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a014_TPMSSoftWarning : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a015_TPMSOverPressureWarning : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a016_TPMSTemperatureWarning : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a017_TPMSSystemFault : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a018_SlipStartOn : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a019_ParkBrakeFault : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a020_SteeringReduced : 19|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a021_RearSeatbeltUnbuckled : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a022_ApeFusesEtc : 21|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a023_CellInternetCheckFailed : 22|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a024_WifiInternetCheckFailed : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a025_WifiOnlineCheckFailed : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a026_ModemResetLoopDetected : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a027_AutoSteerMIA : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a028_FrontTrunkPopupClosed : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a029_ModemMIA : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a030_ModemVMCrash : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a031_BrakeFluidLow : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a032_CellModemRecoveryResets : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a033_ApTrialExpired : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a034_WakeupProblem : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a035_AudioWatchdogKernelError : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a036_AudioWatchdogHfpError : 35|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a037_AudioWatchdogXrunStormEr : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a038_AudioWatchdogA2bI2cLocku : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a039_AudioA2bNeedRediscovery : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a040_HomelinkTransmit : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a041_AudioDmesgXrun : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a042_AudioDmesgRtThrottling : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a043_InvalidMapDataOverride : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a044_AudioDmesgDspException : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a045_ECallNeedsService : 44|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a046_BackupCameraStreamError : 45|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a047_CellRoamingDisallowed : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a048_AudioPremiumAmpCheckFail : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a049_BrakeShiftRequired : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a050_BackupCameraIPUTimeout : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a051_BackupCameraFrameTimeout : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a052_KernelPanicReported : 51|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a053_QtCarExitError : 52|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a054_AudioBoostPowerBad : 53|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a055_ManualECallDisabled : 54|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a056_ManualECallButtonDisconn : 55|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a057_CellAntennaDisconnected : 56|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a058_GPSAntennaDisconnected : 57|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a059_ECallSpeakerDisconnected : 58|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a060_ECallMicDisconnected : 59|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a061_SIMTestFailed : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a062_ENSTestFailed : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a063_CellularTestFailed : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_a064_ModemFirmwareTestFailed : 63|1@1+ (1,0) [0|1] """"  Receiver

BO_ 322 ID142VCLEFT_liftgateStatus: 8 VehicleBus
 SG_ VCLEFT_liftgateStatusIndex M : 0|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCLEFT_liftgateLatchRequest m0 : 61|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_liftgateMvmntNotAllowedCo m0 : 14|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCLEFT_liftgatePhysicalChimeRequ m0 : 21|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_liftgatePosition m1 : 21|7@1- (1,46) [-5|95] ""deg""  Receiver
 SG_ VCLEFT_liftgatePositionCalibrate m0 : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_liftgateRequestSource m0 : 7|3@1+ (1,0) [0|7] """"  Receiver
 SG_ VCLEFT_liftgateSpeed m1 : 28|10@1- (0.1,0) [-30|30] ""deg/s""  Receiver
 SG_ VCLEFT_liftgateState m0 : 3|4@1+ (1,0) [0|12] """"  Receiver
 SG_ VCLEFT_liftgateStoppingCondition m0 : 10|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCLEFT_liftgateStrutCurrent m1 : 11|10@1- (0.1,0) [-30|30] ""A""  Receiver
 SG_ VCLEFT_liftgateStrutDutyCycle m1 : 3|8@1- (1,0) [-100|100] ""%""  Receiver
 SG_ VCLEFT_liftgateUIChimeRequest m0 : 18|3@1+ (1,0) [0|4] """"  Receiver

BO_ 325 ID145ESP_status: 8 ChassisBus
 SG_ ESP_absBrakeEvent2 : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_absFaultLamp : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_brakeApply : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_brakeDiscWipingActive : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_brakeLamp : 21|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_brakeTorqueTarget : 51|13@1+ (2,0) [0|16382] ""Nm""  Receiver
 SG_ ESP_btcTargetState : 38|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_cdpStatus : 34|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_driverBrakeApply : 29|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_ebdFaultLamp : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_ebrStandstillSkid : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_ebrStatus : 49|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_espFaultLamp : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_espLampFlash : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_espModeActive : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_hydraulicBoostEnabled : 19|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_lateralAccelQF : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_longitudinalAccelQF : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_ptcTargetState : 36|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_stabilityControlSts2 : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_statusChecksum : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ ESP_statusCounter : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ ESP_steeringAngleQF : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_yawRateQF : 26|1@1+ (1,0) [0|1] """"  Receiver

BO_ 470 ID1D6DI_limits: 5 VehicleBus
 SG_ DI_limitBaseSpeed : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitClutch : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitDcCapTemp : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitDeltaFluidTemp : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitDiff : 22|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitDischargePower : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitDriveTorque : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitGracefulPowerOff : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitIBat : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitIGBTJunctTemp : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitInverterTemp : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitLimpMode : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitMotorCurrent : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitMotorSpeed : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitMotorVoltage : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitObstacleDetection : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitOilPumpFluidTemp : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitPCBTemp : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitPoleTemp : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitRegenPower : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitRegenTorque : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitRotorTemp : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitShift : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitShockTorque : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitStatorFrequency : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitStatorTemp : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitTCDrive : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitTCRegen : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitVBatHigh : 19|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitVBatLow : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitVehicleSpeed : 21|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitdcLinkCapTemp : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limithvDcCableTemp : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitnegDcBusbarTemp : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitphaseOutBusBarWeldTemp : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitphaseOutBusbarTemp : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitphaseOutLugTemp : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_limitposDcBusbarTemp : 35|1@1+ (1,0) [0|1] """"  Receiver

BO_ 522 ID20AHVP_contactorState: 6 VehicleBus
 SG_ HVP_dcLinkAllowedToEnergize : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_fcContNegativeAuxOpen : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_fcContNegativeState : 12|3@1+ (1,0) [0|7] """"  Receiver
 SG_ HVP_fcContPositiveAuxOpen : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_fcContPositiveState : 16|3@1+ (1,0) [0|7] """"  Receiver
 SG_ HVP_fcContactorSetState : 19|4@1+ (1,0) [0|9] """"  Receiver
 SG_ HVP_fcCtrsClosingAllowed : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_fcCtrsOpenNowRequested : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_fcCtrsOpenRequested : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_fcCtrsRequestStatus : 24|2@1+ (1,0) [0|2] """"  Receiver
 SG_ HVP_fcCtrsResetRequestRequired : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_fcLinkAllowedToEnergize : 44|2@1+ (1,0) [0|2] """"  Receiver
 SG_ HVP_hvilStatus : 40|4@1+ (1,0) [0|9] """"  Receiver
 SG_ HVP_packContNegativeState : 0|3@1+ (1,0) [0|7] """"  Receiver
 SG_ HVP_packContPositiveState : 3|3@1+ (1,0) [0|7] """"  Receiver
 SG_ HVP_packContactorSetState : 8|4@1+ (1,0) [0|9] """"  Receiver
 SG_ HVP_packCtrsClosingAllowed : 35|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_packCtrsOpenNowRequested : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_packCtrsOpenRequested : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_packCtrsRequestStatus : 30|2@1+ (1,0) [0|2] """"  Receiver
 SG_ HVP_packCtrsResetRequestRequired : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_pyroTestInProgress : 37|1@1+ (1,0) [0|1] """"  Receiver

BO_ 526 ID20EPARK_sdiFront: 8 ChassisBus
 SG_ PARK_sdiFrontChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ PARK_sdiFrontCounter : 54|2@1+ (1,0) [0|3] """"  Receiver
 SG_ PARK_sdiSensor1RawDistData : 0|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor2RawDistData : 9|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor3RawDistData : 18|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor4RawDistData : 27|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor5RawDistData : 36|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor6RawDistData : 45|9@1+ (1,0) [0|511] ""cm""  Receiver

BO_ 537 ID219VCSEC_TPMSData: 5 ChassisBus
 SG_ VCSEC_TPMSDataIndex M : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCSEC_TPMSBatVoltage0 m0 : 24|8@1+ (0.01,1.5) [1.5|4.04] ""V""  Receiver
 SG_ VCSEC_TPMSBatVoltage1 m1 : 24|8@1+ (0.01,1.5) [1.5|4.04] ""V""  Receiver
 SG_ VCSEC_TPMSBatVoltage2 m2 : 24|8@1+ (0.01,1.5) [1.5|4.04] ""V""  Receiver
 SG_ VCSEC_TPMSBatVoltage3 m3 : 24|8@1+ (0.01,1.5) [1.5|4.04] ""V""  Receiver
 SG_ VCSEC_TPMSLocation0 m0 : 32|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCSEC_TPMSLocation1 m1 : 32|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCSEC_TPMSLocation2 m2 : 32|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCSEC_TPMSLocation3 m3 : 32|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCSEC_TPMSPressure0 m0 : 8|8@1+ (0.025,0) [0|6.35] ""bar""  Receiver
 SG_ VCSEC_TPMSPressure1 m1 : 8|8@1+ (0.025,0) [0|6.35] ""bar""  Receiver
 SG_ VCSEC_TPMSPressure2 m2 : 8|8@1+ (0.025,0) [0|6.35] ""bar""  Receiver
 SG_ VCSEC_TPMSPressure3 m3 : 8|8@1+ (0.025,0) [0|6.35] ""bar""  Receiver
 SG_ VCSEC_TPMSTemperature0 m0 : 16|8@1+ (1,-40) [-40|214] ""C""  Receiver
 SG_ VCSEC_TPMSTemperature1 m1 : 16|8@1+ (1,-40) [-40|214] ""C""  Receiver
 SG_ VCSEC_TPMSTemperature2 m2 : 16|8@1+ (1,-40) [-40|214] ""C""  Receiver
 SG_ VCSEC_TPMSTemperature3 m3 : 16|8@1+ (1,-40) [-40|214] ""C""  Receiver

BO_ 516 ID204PCS_chgStatus: 8 VehicleBus
 SG_ PCS_chargeShutdownRequest : 57|2@1+ (1,0) [0|2] """"  Receiver
 SG_ PCS_chgInstantAcPowerAvailable : 16|8@1+ (0.1,0) [0|20] ""kW""  Receiver
 SG_ PCS_chgMainState : 0|4@1+ (1,0) [0|9] """"  Receiver
 SG_ PCS_chgMaxAcPowerAvailable : 24|8@1+ (0.1,0) [0|20] ""kW""  Receiver
 SG_ PCS_chgPHAEnable : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ PCS_chgPHALineCurrentRequest : 32|8@1+ (0.1,0) [0|20] ""A""  Receiver
 SG_ PCS_chgPHBEnable : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ PCS_chgPHBLineCurrentRequest : 40|8@1+ (0.1,0) [0|20] ""A""  Receiver
 SG_ PCS_chgPHCEnable : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ PCS_chgPHCLineCurrentRequest : 48|8@1+ (0.1,0) [0|20] ""A""  Receiver
 SG_ PCS_chgPwmEnableLine : 56|1@1+ (1,0) [0|1] """"  Receiver
 SG_ PCS_gridConfig : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ PCS_hvChargeStatus : 4|2@1+ (1,0) [0|3] """"  Receiver
 SG_ PCS_hwVariantType : 59|2@1+ (1,0) [0|3] """"  Receiver

BO_ 554 ID22AHVP_pcsControl: 4 VehicleBus
 SG_ HVP_dcLinkVoltageFiltered : 20|11@1- (1,0) [-550|550] ""V""  Receiver
 SG_ HVP_dcLinkVoltageRequest : 0|16@1- (0.1,0) [-550|550] ""V""  Receiver
 SG_ HVP_pcsChargeHwEnabled : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ HVP_pcsControlRequest : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ HVP_pcsDcdcHwEnabled : 19|1@1+ (1,0) [0|1] """"  Receiver

BO_ 562 ID232BMS_contactorRequest: 8 VehicleBus
 SG_ BMS_ensShouldBeActiveForDrive : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_fcContactorRequest : 0|3@1+ (1,0) [0|5] """"  Receiver
 SG_ BMS_fcLinkOkToEnergizeRequest : 32|2@1+ (1,0) [0|2] """"  Receiver
 SG_ BMS_gpoHasCompleted : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_internalHvilSenseV : 16|16@1+ (0.001,0) [0|65.534] ""V""  Receiver
 SG_ BMS_packContactorRequest : 3|3@1+ (1,0) [0|5] """"  Receiver
 SG_ BMS_pcsPwmDisable : 8|1@1+ (1,0) [0|1] """"  Receiver

BO_ 627 ID273UI_vehicleControl: 8 VehicleBus
 SG_ UI_accessoryPowerRequest : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_alarmEnabled : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_ambientLightingEnabled : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autoFoldMirrorsOn : 52|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autoHighBeamEnabled : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_childDoorLockOn : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_displayBrightnessLevel : 32|8@1+ (0.5,0) [0|127] ""%""  Receiver
 SG_ UI_domeLightSwitch : 59|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_driveStateRequest : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_frontFogSwitch : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_frontLeftSeatHeatReq : 42|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_frontRightSeatHeatReq : 44|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_frunkRequest : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_globalUnlockOn : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_honkHorn : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_intrusionSensorOn : 21|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_lockRequest : 17|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_mirrorDipOnReverse : 53|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_mirrorFoldRequest : 24|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_mirrorHeatRequest : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_powerOff : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rearCenterSeatHeatReq : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_rearFogSwitch : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rearLeftSeatHeatReq : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_rearRightSeatHeatReq : 50|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_rearWindowLockout : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_remoteClosureRequest : 54|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_remoteStartRequest : 27|3@1+ (1,0) [0|4] """"  Receiver
 SG_ UI_seeYouHomeLightingOn : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_steeringBacklightEnabled : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_steeringButtonMode : 9|3@1+ (1,0) [0|5] """"  Receiver
 SG_ UI_stop12vSupport : 22|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_summonActive : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_unlockOnPark : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_walkAwayLock : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_walkUpUnlock : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_wiperMode : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_wiperRequest : 56|3@1+ (1,0) [0|6] """"  Receiver

BO_ 637 ID27DCP_dcChargeLimits: 8 VehicleBus
 SG_ CP_evseInstantDcCurrentLimit : 52|12@1+ (0.146484,0) [0|599.854] ""A""  Receiver
 SG_ CP_evseMaxDcCurrentLimit : 0|13@1+ (0.0732422,0) [0|599.927] ""A""  Receiver
 SG_ CP_evseMaxDcVoltageLimit : 26|13@1+ (0.0732422,0) [0|599.927] ""V""  Receiver
 SG_ CP_evseMinDcCurrentLimit : 13|13@1+ (0.0732422,0) [0|599.927] ""A""  Receiver
 SG_ CP_evseMinDcVoltageLimit : 39|13@1+ (0.0732422,0) [0|599.927] ""V""  Receiver

BO_ 701 ID2BDCP_dcPowerLimits: 4 VehicleBus
 SG_ CP_evseInstantDcPowerLimit : 0|13@1+ (0.0622559,0) [0|509.938] ""kW""  Receiver
 SG_ CP_evseMaxDcPowerLimit : 16|13@1+ (0.0622559,0) [0|509.938] ""kW""  Receiver

BO_ 1066 ID42AVCSEC_TPMSConnectionData: 8 VehicleBus
 SG_ VCSEC_TPMSConnectionTypeCurrent0 : 11|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSConnectionTypeCurrent1 : 26|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSConnectionTypeCurrent2 : 41|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSConnectionTypeCurrent3 : 56|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSConnectionTypeDesired0 : 13|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSConnectionTypeDesired1 : 28|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSConnectionTypeDesired2 : 43|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSConnectionTypeDesired3 : 58|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCSEC_TPMSRSSI0 : 3|8@1- (1,0) [-127|0] ""dBm""  Receiver
 SG_ VCSEC_TPMSRSSI1 : 18|8@1- (1,0) [-127|0] ""dBm""  Receiver
 SG_ VCSEC_TPMSRSSI2 : 33|8@1- (1,0) [-127|0] ""dBm""  Receiver
 SG_ VCSEC_TPMSRSSI3 : 48|8@1- (1,0) [-127|0] ""dBm""  Receiver
 SG_ VCSEC_TPMSSensorState0 : 0|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCSEC_TPMSSensorState1 : 15|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCSEC_TPMSSensorState2 : 30|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCSEC_TPMSSensorState3 : 45|3@1+ (1,0) [0|4] """"  Receiver

BO_ 558 ID22EPARK_sdiRear: 8 VehicleBus
 SG_ PARK_sdiRearChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ PARK_sdiRearCounter : 54|2@1+ (1,0) [0|3] """"  Receiver
 SG_ PARK_sdiSensor10RawDistData : 27|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor11RawDistData : 36|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor12RawDistData : 45|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor7RawDistData : 0|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor8RawDistData : 9|9@1+ (1,0) [0|511] ""cm""  Receiver
 SG_ PARK_sdiSensor9RawDistData : 18|9@1+ (1,0) [0|511] ""cm""  Receiver

BO_ 568 ID238UI_driverAssistMapData: 8 ChassisBus
 SG_ UI_acceptBottsDots : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autosteerRestricted : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_controlledAccess : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_countryCode : 16|10@1+ (1,0) [0|1023] """"  Receiver
 SG_ UI_gpsRoadMatch : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_inSuperchargerGeofence : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_mapDataChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ UI_mapDataCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_mapSpeedLimit : 8|5@1+ (1,0) [0|31] """"  Receiver
 SG_ UI_mapSpeedLimitDependency : 0|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_mapSpeedLimitType : 13|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_mapSpeedUnits : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_navRouteActive : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_nextBranchDist : 32|5@1+ (10,0) [0|300] ""m""  Receiver
 SG_ UI_nextBranchLeftOffRamp : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_nextBranchRightOffRamp : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_parallelAutoparkEnabled : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_perpendicularAutoparkEnabled : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_pmmEnabled : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectAutosteer : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectHPP : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectHandsOn : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectLeftFreeSpace : 44|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectLeftLane : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectNav : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectRightFreeSpace : 45|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_rejectRightLane : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_roadClass : 3|3@1+ (1,0) [0|6] """"  Receiver
 SG_ UI_scaEnabled : 51|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_streetCount : 26|2@1+ (1,0) [0|3] """"  Receiver

BO_ 569 ID239DAS_lanes: 8 ChassisBus
 SG_ DAS_lanesCounter : 60|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_leftFork : 52|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_leftLaneExists : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_leftLineUsage : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_rightFork : 54|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_rightLaneExists : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_rightLineUsage : 50|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_virtualLaneC0 : 16|8@1+ (0.035,-3.5) [-3.5|3.5] ""m""  Receiver
 SG_ DAS_virtualLaneC1 : 24|8@1+ (0.0016,-0.2) [-0.2|0.2] ""rad""  Receiver
 SG_ DAS_virtualLaneC2 : 32|8@1+ (2E-005,-0.0025) [-0.0025|0.0025] ""m-1""  Receiver
 SG_ DAS_virtualLaneC3 : 40|8@1+ (2.4E-007,-3E-005) [-3E-005|3E-005] ""m-2""  Receiver
 SG_ DAS_virtualLaneViewRange : 8|8@1+ (1,0) [0|160] ""m""  Receiver
 SG_ DAS_virtualLaneWidth : 4|4@1+ (0.3125,2) [2|6.6875] ""m""  Receiver

BO_ 586 ID24ADAS_visualDebug: 8 ChassisBus
 SG_ DAS_accSmartSpeedActive : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_accSmartSpeedState : 49|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DAS_autosteerBottsDotsUsage : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_autosteerHPPUsage : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_autosteerHealthAnomalyLevel : 18|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DAS_autosteerHealthState : 21|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_autosteerModelUsage : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_autosteerNavigationUsage : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_autosteerVehiclesUsage : 4|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_behaviorType : 56|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_devAppInterfaceEnabled : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_lastAutosteerAbortReason : 32|6@1+ (1,0) [0|34] """"  Receiver
 SG_ DAS_lastLinePreferenceReason : 24|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_navAvailable : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_navDistance : 40|8@1+ (100,0) [0|25500] ""km""  Receiver
 SG_ DAS_offsetSide : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_plannerState : 28|4@1+ (1,0) [0|7] """"  Receiver
 SG_ DAS_rearLeftVehDetectedCurrent : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_rearLeftVehDetectedTrip : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_rearRightVehDetectedTrip : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_rearVehDetectedThisCycle : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_roadSurfaceType : 16|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_ulcInProgress : 52|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_ulcType : 58|2@1+ (1,0) [0|2] """"  Receiver

BO_ 603 ID25BAPP_environment: 1 ChassisBus
 SG_ APP_environmentRainy : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ APP_environmentSnowy : 1|1@1+ (1,0) [0|1] """"  Receiver

BO_ 605 ID25DCP_status: 8 VehicleBus
 SG_ CP_UHF_controlState : 38|4@1+ (1,0) [0|10] """"  Receiver
 SG_ CP_UHF_handleFound : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_apsVoltage : 24|8@1+ (0.0715686,0) [0|18.249] ""V""  Receiver
 SG_ CP_chargeCablePresent : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_chargeCableSecured : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_chargeCableState : 14|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_chargeDoorOpen : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_chargeDoorOpenUI : 51|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_coldWeatherMode : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_coverClosed : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_doorButtonPressed : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_doorControlState : 11|3@1+ (1,0) [0|6] """"  Receiver
 SG_ CP_doorOpenRequested : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_faultLineSensed : 44|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_hvInletExposed : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_inductiveDoorState : 45|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CP_inductiveSensorState : 48|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CP_insertEnableLine : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_latch2ControlState : 19|3@1+ (1,0) [0|5] """"  Receiver
 SG_ CP_latch2State : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_latchControlState : 16|3@1+ (1,0) [0|5] """"  Receiver
 SG_ CP_latchEngaged : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_latchState : 5|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_ledColor : 33|4@1+ (1,0) [0|10] """"  Receiver
 SG_ CP_numAlertsSet : 53|7@1+ (1,0) [0|127] """"  Receiver
 SG_ CP_permanentPowerRequest : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_swcanRelayClosed : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_type : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_vehicleUnlockRequest : 52|1@1+ (1,0) [0|1] """"  Receiver

BO_ 669 ID29DCP_dcChargeStatus: 4 VehicleBus
 SG_ CP_evseOutputDcCurrent : 0|15@1- (0.0732467,0) [-1200|1200] ""A""  Receiver
 SG_ CP_evseOutputDcCurrentStale : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_evseOutputDcVoltage : 16|13@1+ (0.0732422,0) [0|599.927] ""V""  Receiver

BO_ 692 ID2B4PCS_dcdcRailStatus: 5 VehicleBus
 SG_ PCS_dcdcHvBusVolt : 10|12@1+ (0.146484,0) [0|599.854] ""V""  Receiver
 SG_ PCS_dcdcLvBusVolt : 0|10@1+ (0.0390625,0) [0|39.9609] ""V""  Receiver
 SG_ PCS_dcdcLvOutputCurrent : 24|12@1+ (0.1,0) [0|400] ""A""  Receiver

BO_ 697 ID2B9DAS_control: 8 ChassisBus
 SG_ DAS_accState : 12|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_accelMax : 44|9@1+ (0.04,-15) [-15|5.44] ""m/s^2""  Receiver
 SG_ DAS_accelMin : 35|9@1+ (0.04,-15) [-15|5.44] ""m/s^2""  Receiver
 SG_ DAS_aebEvent : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_controlChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DAS_controlCounter : 53|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DAS_jerkMax : 27|8@1+ (0.059,0) [0|15.045] ""m/s^3""  Receiver
 SG_ DAS_jerkMin : 18|9@1+ (0.03,-15.232) [-15.232|0.098] ""m/s^3""  Receiver
 SG_ DAS_setSpeed : 0|12@1+ (0.1,0) [0|409.4] ""kph""  Receiver

BO_ 723 ID2D3UI_solarData: 8 ChassisBus
 SG_ UI_isSunUp : 25|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_minsToSunrise : 56|8@1+ (10,0) [0|2550] ""min""  Receiver
 SG_ UI_minsToSunset : 48|8@1+ (10,0) [0|2550] ""min""  Receiver
 SG_ UI_screenPCBTemperature : 40|8@1- (0.5,40) [-20|100] ""C""  Receiver
 SG_ UI_solarAzimuthAngle : 0|16@1- (1,0) [-32767|32767] ""deg""  Receiver
 SG_ UI_solarAzimuthAngleCarRef : 16|9@1- (1,0) [-256|254] ""deg""  Receiver
 SG_ UI_solarElevationAngle : 32|8@1- (1,0) [-128|126] ""deg""  Receiver

BO_ 777 ID309DAS_object: 8 VehicleBus
 SG_ DAS_objectId M : 0|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_cutinVehDx m3 : 8|8@1+ (0.5,0) [0|127] ""m""  Receiver
 SG_ DAS_cutinVehDy m3 : 20|7@1+ (0.35,-22.05) [-22.05|22.4] ""m""  Receiver
 SG_ DAS_cutinVehHeading m5 : 56|8@1+ (0.0245437,-3.14159) [-3.14159|3.09251] ""rad""  Receiver
 SG_ DAS_cutinVehId m3 : 27|7@1+ (1,0) [0|127] """"  Receiver
 SG_ DAS_cutinVehRelevantForControl m3 : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_cutinVehType m3 : 3|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_cutinVehVxRel m3 : 16|4@1+ (4,-30) [-30|26] ""m/s""  Receiver
 SG_ DAS_leadVeh2Dx m0 : 39|8@1+ (0.5,0) [0|127] ""m""  Receiver
 SG_ DAS_leadVeh2Dy m0 : 51|7@1+ (0.35,-22.05) [-22.05|22.4] ""m""  Receiver
 SG_ DAS_leadVeh2Heading m5 : 16|8@1+ (0.0245437,-3.14159) [-3.14159|3.09251] ""rad""  Receiver
 SG_ DAS_leadVeh2Id m0 : 58|6@1+ (1,0) [0|63] """"  Receiver
 SG_ DAS_leadVeh2RelevantForControl m0 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_leadVeh2Type m0 : 34|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_leadVeh2VxRel m0 : 47|4@1+ (4,-30) [-30|26] ""m/s""  Receiver
 SG_ DAS_leadVehDx m0 : 8|8@1+ (0.5,0) [0|127] ""m""  Receiver
 SG_ DAS_leadVehDy m0 : 20|7@1+ (0.35,-22.05) [-22.05|22.4] ""m""  Receiver
 SG_ DAS_leadVehHeading m5 : 8|8@1+ (0.0245437,-3.14159) [-3.14159|3.09251] ""rad""  Receiver
 SG_ DAS_leadVehId m0 : 27|7@1+ (1,0) [0|127] """"  Receiver
 SG_ DAS_leadVehRelevantForControl m0 : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_leadVehType m0 : 3|3@1+ (1,0) [0|6] """"  Receiver
 SG_ DAS_leadVehVxRel m0 : 16|4@1+ (4,-30) [-30|26] ""m/s""  Receiver
 SG_ DAS_leftVeh2Dx m1 : 39|8@1+ (0.5,0) [0|127] ""m""  Receiver
 SG_ DAS_leftVeh2Dy m1 : 51|7@1+ (0.35,-22.05) [-22.05|22.4] ""m""  Receiver
 SG_ DAS_leftVeh2Heading m5 : 32|8@1+ (0.0245437,-3.14159) [-3.14159|3.09251] ""rad""  Receiver
 SG_ DAS_leftVeh2Id m1 : 58|6@1+ (1,0) [0|63] """"  Receiver
 SG_ DAS_leftVeh2RelevantForControl m1 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_leftVeh2Type m1 : 34|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_leftVeh2VxRel m1 : 47|4@1+ (4,-30) [-30|26] ""m/s""  Receiver
 SG_ DAS_leftVehDx m1 : 8|8@1+ (0.5,0) [0|127] ""m""  Receiver
 SG_ DAS_leftVehDy m1 : 20|7@1+ (0.35,-22.05) [-22.05|22.4] ""m""  Receiver
 SG_ DAS_leftVehHeading m5 : 24|8@1+ (0.0245437,-3.14159) [-3.14159|3.09251] ""rad""  Receiver
 SG_ DAS_leftVehId m1 : 27|7@1+ (1,0) [0|127] """"  Receiver
 SG_ DAS_leftVehRelevantForControl m1 : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_leftVehType m1 : 3|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_leftVehVxRel m1 : 16|4@1+ (4,-30) [-30|26] ""m/s""  Receiver
 SG_ DAS_rightVeh2Dx m2 : 39|8@1+ (0.5,0) [0|127] ""m""  Receiver
 SG_ DAS_rightVeh2Dy m2 : 51|7@1+ (0.35,-22.05) [-22.05|22.4] ""m""  Receiver
 SG_ DAS_rightVeh2Heading m5 : 48|8@1+ (0.0245437,-3.14159) [-3.14159|3.09251] ""rad""  Receiver
 SG_ DAS_rightVeh2Id m2 : 58|6@1+ (1,0) [0|63] """"  Receiver
 SG_ DAS_rightVeh2RelevantForControl m2 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_rightVeh2Type m2 : 34|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_rightVeh2VxRel m2 : 47|4@1+ (4,-30) [-30|26] ""m/s""  Receiver
 SG_ DAS_rightVehDx m2 : 8|8@1+ (0.5,0) [0|127] ""m""  Receiver
 SG_ DAS_rightVehDy m2 : 20|7@1+ (0.35,-22.05) [-22.05|22.4] ""m""  Receiver
 SG_ DAS_rightVehHeading m5 : 40|8@1+ (0.0245437,-3.14159) [-3.14159|3.09251] ""rad""  Receiver
 SG_ DAS_rightVehId m2 : 27|7@1+ (1,0) [0|127] """"  Receiver
 SG_ DAS_rightVehRelevantForControl m2 : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_rightVehType m2 : 3|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_rightVehVxRel m2 : 16|4@1+ (4,-30) [-30|26] ""m/s""  Receiver
 SG_ DAS_roadSignArrow m4 : 27|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DAS_roadSignColor m4 : 3|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DAS_roadSignControlActive m4 : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_roadSignId m4 : 6|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DAS_roadSignOrientation m4 : 30|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_roadSignSource m4 : 25|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_roadSignStopLineDist m4 : 14|10@1+ (0.2,-20) [-20|184.4] ""m""  Receiver

BO_ 905 ID389DAS_status2: 8 ChassisBus
 SG_ DAS_ACC_report : 26|5@1+ (1,0) [0|24] """"  Receiver
 SG_ DAS_accSpeedLimit : 0|10@1+ (0.2,0) [0|204.6] ""mph""  Receiver
 SG_ DAS_activationFailureStatus : 14|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_csaState : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_driverInteractionLevel : 38|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_longCollisionWarning : 48|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_pmmCameraFaultReason : 24|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_pmmLoggingRequest : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_pmmObstacleSeverity : 10|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DAS_pmmRadarFaultReason : 19|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_pmmSysFaultReason : 21|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DAS_pmmUltrasonicsFaultReason : 16|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DAS_ppOffsetDesiredRamp : 40|8@1+ (0.01,-1.28) [-1.28|1.27] ""m""  Receiver
 SG_ DAS_radarTelemetry : 34|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_relaxCruiseLimits : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_robState : 36|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_status2Checksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DAS_status2Counter : 52|4@1+ (1,0) [0|15] """"  Receiver

BO_ 921 ID399DAS_status: 8 ChassisBus
 SG_ DAS_autoLaneChangeState : 46|5@1+ (1,0) [0|31] """"  Receiver
 SG_ DAS_autoParked : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_autoparkReady : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_autoparkWaitingForBrake : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_autopilotHandsOnState : 42|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_autopilotState : 0|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_blindSpotRearLeft : 4|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_blindSpotRearRight : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_fleetSpeedState : 40|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_forwardCollisionWarning : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_fusedSpeedLimit : 8|5@1+ (5,0) [0|150] ""kph/mph""  Receiver
 SG_ DAS_heaterState : 21|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_laneDepartureWarning : 37|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_lssState : 29|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DAS_sideCollisionAvoid : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_sideCollisionInhibit : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_sideCollisionWarning : 34|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_statusChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DAS_statusCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_summonAvailable : 51|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_summonClearedGate : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_summonFwdLeashReached : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_summonObstacle : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_summonRvsLeashReached : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_suppressSpeedWarning : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_visionOnlySpeedLimit : 16|5@1+ (5,0) [0|150] ""kph/mph""  Receiver

BO_ 925 ID39DIBST_status: 5 ChassisBus
 SG_ IBST_driverBrakeApply : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ IBST_iBoosterStatus : 12|3@1+ (1,0) [0|6] """"  Receiver
 SG_ IBST_internalState : 18|3@1+ (1,0) [0|6] """"  Receiver
 SG_ IBST_sInputRodDriver : 21|12@1+ (0.015625,-5) [-5|47] ""mm""  Receiver
 SG_ IBST_statusChecksum : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ IBST_statusCounter : 8|4@1+ (1,0) [0|15] """"  Receiver

BO_ 929 ID3A1VCFRONT_vehicleStatus: 8 VehicleBus
 SG_ VCFRONT_12vStatusForDrive : 14|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_2RowCenterUnbuckled : 38|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_2RowLeftUnbuckled : 36|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_2RowRightUnbuckled : 40|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_APGlassHeaterState : 2|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_accPlusAvailable : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_batterySupportRequest : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_bmsHvChargeEnable : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_diPowerOnState : 10|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_driverBuckleStatus : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_driverDoorStatus : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_driverIsLeaving : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_driverIsLeavingAnySpeed : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_driverPresent : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_driverUnbuckled : 32|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_ota12VSupportRequest : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_passengerPresent : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_passengerUnbuckled : 34|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_pcs12vVoltageTarget : 16|11@1+ (0.01,0) [0|16] ""V""  Receiver
 SG_ VCFRONT_pcsEFuseVoltage : 42|10@1+ (0.1,0) [0|102.2] ""V""  Receiver
 SG_ VCFRONT_preconditionRequest : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_standbySupplySupported : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_thermalSystemType : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_vehicleStatusChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_vehicleStatusCounter : 52|4@1+ (1,0) [0|15] """"  Receiver

BO_ 985 ID3D9UI_gpsVehicleSpeed: 8 ChassisBus
 SG_ UI_conditionalLimitActive : 55|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_conditionalSpeedLimit : 56|5@1+ (5,0) [0|155] ""kph/mph""  Receiver
 SG_ UI_gpsAntennaDisconnected : 54|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_gpsHDOP : 0|8@1+ (0.1,0) [0|25.5] """"  Receiver
 SG_ UI_gpsNmeaMIA : 53|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_gpsVehicleHeading : 8|16@1+ (0.0078125,0) [0|511.992] ""deg""  Receiver
 SG_ UI_gpsVehicleSpeed : 24|16@1+ (0.00390625,0) [0|250.996] ""kph""  Receiver
 SG_ UI_mapSpeedLimitUnits : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_mppSpeedLimit : 48|5@1+ (5,0) [0|155] ""kph/mph""  Receiver
 SG_ UI_userSpeedOffset : 40|6@1+ (1,-30) [-30|33] ""kph/mph""  Receiver
 SG_ UI_userSpeedOffsetUnits : 47|1@1+ (1,0) [0|1] """"  Receiver

BO_ 994 ID3E2VCLEFT_lightStatus: 7 VehicleBus
 SG_ VCLEFT_FLMapLightStatus : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_FLMapLightSwitchPressed : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_FRMapLightStatus : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_FRMapLightSwitchPressed : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_RLMapLightStatus : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_RLMapLightSwitchPressed : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_RRMapLightStatus : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_RRMapLightSwitchPressed : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_brakeLightStatus : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_brakeTrailerLightStatus : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_dynamicBrakeLightStatus : 44|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCLEFT_fogTrailerLightStatus : 26|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontRideHeight : 28|8@1- (1,0) [-127|127] ""mm""  Receiver
 SG_ VCLEFT_leftTurnTrailerLightStatu : 18|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rearRideHeight : 36|8@1- (1,0) [-127|127] ""mm""  Receiver
 SG_ VCLEFT_reverseTrailerLightStatus : 49|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rideHeightSensorFault : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_rightTrnTrailerLightStatu : 20|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_tailLightOutageStatus : 51|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_tailLightStatus : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_tailTrailerLightStatus : 24|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_trailerDetected : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_turnSignalStatus : 4|2@1+ (1,0) [0|3] """"  Receiver

BO_ 1001 ID3E9DAS_bodyControls: 8 VehicleBus
 SG_ DAS_bodyControlsChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DAS_bodyControlsCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_dynamicBrakeLightRequest : 22|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_hazardLightRequest : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_headlightRequest : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_heaterRequest : 13|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DAS_highLowBeamDecision : 11|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DAS_highLowBeamOffReason : 15|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DAS_turnIndicatorRequest : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DAS_turnIndicatorRequestReason : 18|4@1+ (1,0) [0|12] """"  Receiver
 SG_ DAS_wiperSpeed : 4|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DAS_radarHeaterRequest : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_ahlbOverride : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DAS_mirrorFoldRequest : 25|2@1+ (1,0) [0|0] """"  Receiver

BO_ 1011 ID3F3UI_odo: 3 ChassisBus
 SG_ UI_odometer : 0|24@1+ (0.1,0) [0|1677720] ""km""  Receiver

BO_ 1013 ID3F5VCFRONT_lighting: 8 VehicleBus
 SG_ VCFRONT_DRLLeftStatus : 36|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_DRLRightStatus : 38|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_ambientLightingBrightnes : 8|8@1+ (0.5,0) [0|127] ""%""  Receiver
 SG_ VCFRONT_approachLightingRequest : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_courtesyLightingRequest : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_fogLeftStatus : 40|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_fogRightStatus : 42|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_hazardLightRequest : 4|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCFRONT_hazardSwitchBacklight : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_highBeamLeftStatus : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_highBeamRightStatus : 34|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_highBeamSwitchActive : 58|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_indicatorLeftRequest : 0|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_indicatorRightRequest : 2|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_lowBeamLeftStatus : 28|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_lowBeamRightStatus : 30|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_lowBeamsCalibrated : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_lowBeamsOnForDRL : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_parkLeftStatus : 54|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_parkRightStatus : 56|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_seeYouHomeLightingReq : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_sideMarkersStatus : 44|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_sideRepeaterLeftStatus : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_sideRepeaterRightStatus : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_simLatchingStalk : 59|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_switchLightingBrightness : 16|8@1+ (0.5,0) [0|127] ""%""  Receiver
 SG_ VCFRONT_turnSignalLeftStatus : 50|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_turnSignalRightStatus : 52|2@1+ (1,0) [0|3] """"  Receiver

BO_ 1016 ID3F8UI_driverAssistControl: 8 ChassisBus
 SG_ UI_accFollowDistanceSetting : 45|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_adaptiveSetSpeedEnable : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_alcOffHighwayEnable : 56|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autoSummonEnable : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autopilotControlRequest : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_coastToCoast : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_curvSpeedAdaptDisable : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_curvatureDatabaseOnly : 22|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_dasDeveloper : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_driveOnMapsEnable : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_drivingSide : 40|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_enableBrakeLightPulse : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableClipTelemetry : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableRoadSegmentTelemetry : 44|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableTripTelemetry : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableVinAssociation : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableVisionOnlyStops : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_exceptionListEnable : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_followNavRouteEnable : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_fuseHPPDisable : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_fuseLanesDisable : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_fuseVehiclesDisable : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_handsOnRequirementDisable : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_hasDriveOnNav : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_lssElkEnabled : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_lssLdwEnabled : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_lssLkaEnabled : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_roadCheckDisable : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_selfParkRequest : 28|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_smartSummonType : 58|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_source3D : 61|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_summonEntryType : 26|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_summonExitType : 24|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_summonHeartbeat : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_summonReverseDist : 32|6@1+ (1,0) [0|63] """"  Receiver
 SG_ UI_ulcBlindSpotConfig : 52|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_ulcOffHighway : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_ulcSpeedConfig : 50|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_ulcStalkConfirm : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_undertakeAssistEnable : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_validationLoop : 57|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_visionSpeedType : 20|2@1+ (1,0) [0|3] """"  Receiver

BO_ 1021 ID3FDUI_autopilotControl: 8 ChassisBus
 SG_ UI_autopilotControlIndex M : 0|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_apmv3Branch m1 : 40|3@1+ (1,0) [0|5] """"  Receiver
 SG_ UI_applyEceR79 m1 : 19|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_blindspotDistance m0 : 15|3@1+ (1,0) [0|5] """"  Receiver
 SG_ UI_blindspotMinSpeed m0 : 11|4@1+ (1,0) [0|10] """"  Receiver
 SG_ UI_blindspotTTC m0 : 18|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_disableBackup m1 : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableFisheye m1 : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableLeftPillar m1 : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableLeftRepeater m1 : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableMain m1 : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableNarrow m1 : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableRadar m1 : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableRightPillar m1 : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_disableRightRepeater m1 : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_donAlcProgGoreAbortThres m0 : 29|4@1+ (1,0) [0|9] """"  Receiver
 SG_ UI_donDisableAutoWiperDuration m0 : 4|3@1+ (1,0) [0|6] """"  Receiver
 SG_ UI_donDisableCutin m0 : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_donDisableOnAutoWiperSpeed m0 : 7|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_donMinGoreWidthForAbortMap m0 : 25|4@1+ (1,0) [0|11] """"  Receiver
 SG_ UI_donMinGoreWidthForAbortNotMap m0 : 33|4@1+ (1,0) [0|11] """"  Receiver
 SG_ UI_donStopEndOfRampBuffer m0 : 21|3@1+ (1,0) [0|4] """"  Receiver
 SG_ UI_driverMonitorConfirmation m1 : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableAutopilotStopWarning m1 : 44|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableCabinCamera m1 : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableCabinCameraTelemetry m1 : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_enableMapStops m1 : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_factorySummonEnable m1 : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_fsdStopsControlEnabled m0 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_fsdVisualizationEnabled m0 : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_hardCoreSummon m1 : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_homelinkNearby m0 : 45|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_hovEnabled m0 : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_noStalkConfirmAlertChime m1 : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_noStalkConfirmAlertHaptic m1 : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_showLaneGraph m1 : 45|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_showTrackLabels m1 : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_ulcSnooze m1 : 36|1@1+ (1,0) [0|1] """"  Receiver

BO_ 615 ID267DI_vehicleEstimates: 8 VehicleBus
 SG_ DI_gradeEst : 33|7@1- (1,0) [-40|40] ""%""  Receiver
 SG_ DI_gradeEstInternal : 48|7@1- (1,0) [-40|40] ""%""  Receiver
 SG_ DI_mass : 0|10@1+ (5,1900) [1900|7010] ""kg""  Receiver
 SG_ DI_massConfidence : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_relativeTireTreadDepth : 16|6@1- (0.2,0) [-6.2|6.2] ""mm""  Receiver
 SG_ DI_steeringAngleOffset : 56|8@1- (0.2,0) [-25.6|25.4] ""Deg""  Receiver
 SG_ DI_tireFitment : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DI_trailerDetected : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_vehicleEstimatesChecksum : 40|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DI_vehicleEstimatesCounter : 13|3@1+ (1,0) [0|7] """"  Receiver

BO_ 642 ID282VCLEFT_hvacBlowerFeedback: 8 VehicleBus
 SG_ VCLEFT_blowerIndex M : 0|2@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerCBCCtrlState m1 : 58|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_hvacBlowerCBCEstState m1 : 54|4@1+ (1,0) [0|14] """"  Receiver
 SG_ VCLEFT_hvacBlowerEnabled m0 : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerFETTemp m0 : 44|7@1+ (1.5,-40) [-40|149] ""C""  Receiver
 SG_ VCLEFT_hvacBlowerFault m0 : 59|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerIPhase0 m1 : 10|8@1+ (0.2,0) [0|50] ""A""  Receiver
 SG_ VCLEFT_hvacBlowerIPhase1 m1 : 18|8@1+ (0.2,0) [0|50] ""A""  Receiver
 SG_ VCLEFT_hvacBlowerIPhase2 m1 : 26|8@1+ (0.2,0) [0|50] ""A""  Receiver
 SG_ VCLEFT_hvacBlowerITerm m0 : 51|7@1+ (0.05,-1.5) [-1.5|4.5] ""Nm""  Receiver
 SG_ VCLEFT_hvacBlowerInitd m0 : 58|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerLimitFETTemps m0 : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerOutputDuty m0 : 3|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCLEFT_hvacBlowerPowerOn m0 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerRPMActual m0 : 20|10@1+ (10,0) [0|10000] ""rpm""  Receiver
 SG_ VCLEFT_hvacBlowerRPMTarget m0 : 10|10@1+ (10,0) [0|10000] ""rpm""  Receiver
 SG_ VCLEFT_hvacBlowerRs m1 : 2|8@1+ (0.25,0) [0|60] ""mOhm""  Receiver
 SG_ VCLEFT_hvacBlowerRsOnlineActive m1 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerSpiError m0 : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlowerTorque m0 : 34|10@1+ (0.006,-1.5) [-1.5|4.5] ""Nm""  Receiver
 SG_ VCLEFT_hvacBlower_IO_CBC_HEAD m1 : 34|4@1+ (1,0) [0|15] """"  Receiver
 SG_ VCLEFT_hvacBlower_IO_CBC_Status m1 : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlower_IO_CBC_TAIL m1 : 38|4@1+ (1,0) [0|15] """"  Receiver
 SG_ VCLEFT_hvacBlower_IO_CBC_TAIL_va m1 : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hvacBlower_IO_CBC_numUart m1 : 46|8@1+ (1,0) [0|255] """"  Receiver

BO_ 755 ID2F3UI_hvacRequest: 5 VehicleBus
 SG_ UI_hvacReqACDisable : 22|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_hvacReqAirDistributionMode : 13|3@1+ (1,0) [0|7] """"  Receiver
 SG_ UI_hvacReqBlowerSegment : 16|4@1+ (1,0) [0|11] """"  Receiver
 SG_ UI_hvacReqKeepClimateOn : 33|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_hvacReqManualDefogState : 24|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_hvacReqRecirc : 20|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_hvacReqSecondRowState : 29|3@1+ (1,0) [0|4] """"  Receiver
 SG_ UI_hvacReqTempSetpointLeft : 0|5@1+ (0.5,15) [15|28] ""C""  Receiver
 SG_ UI_hvacReqTempSetpointRight : 8|5@1+ (0.5,15) [15|28] ""C""  Receiver
 SG_ UI_hvacReqUserPowerState : 26|3@1+ (1,0) [0|4] """"  Receiver
 SG_ UI_hvacUseModeledDuctTemp : 32|1@1+ (1,0) [0|1] """"  Receiver

BO_ 787 ID313UI_trackModeSettings: 8 VehicleBus
 SG_ UI_trackCmpOverclock : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_trackModeRequest : 0|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_trackModeSettingsChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ UI_trackModeSettingsCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_trackPostCooling : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_trackRotationTendency : 8|8@1+ (0.5,0) [0|100] ""%""  Receiver
 SG_ UI_trackStabilityAssist : 16|8@1+ (0.5,0) [0|100] ""%""  Receiver

BO_ 821 ID335RearDIinfo: 8 VehicleBus
 SG_ DIR_infoIndex M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_FPGA_version m16 : 48|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_appGitHash m17 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ DIR_applicationCrc m13 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIR_assemblyId m11 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_bootGitHash m18 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ DIR_bootloaderCrc m20 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIR_buildConfigurationId m10 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIR_buildType m10 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DIR_componentId m10 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIR_hardwareId m10 : 32|8@1+ (1,0) [0|252] """"  Receiver
 SG_ DIR_infoBootLdUdsProtocolVersion m20 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_oilPumpAppCrc m16 : 16|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIR_oilPumpBuildType m16 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DIR_pcbaId m11 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_platformTyp m19 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_subUsageId m11 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIR_subcomponentGitHash m31 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ DIR_usageId m11 : 32|16@1+ (1,0) [0|65535] """"  Receiver

BO_ 899 ID383VCRIGHT_thsStatus: 8 VehicleBus
 SG_ VCRIGHT_estimatedThsSolarLoad : 53|10@1+ (1,0) [0|1022] ""W/m2""  Receiver
 SG_ VCRIGHT_estimatedVehicleSituatio : 31|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCRIGHT_thsActive : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_thsHumidity : 17|8@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCRIGHT_thsSolarLoadInfrared : 33|10@1+ (1,0) [0|1022] ""W/m2""  Receiver
 SG_ VCRIGHT_thsSolarLoadVisible : 43|10@1+ (1,0) [0|1022] ""W/m2""  Receiver
 SG_ VCRIGHT_thsTemperature : 1|8@1- (1,-40) [-40|150] ""C""  Receiver

BO_ 947 ID3B3UI_vehicleControl2: 4 VehicleBus
 SG_ UI_PINToDriveEnabled : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_PINToDrivePassed : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_UMCUpdateInhibit : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_VCLEFTFeature1 : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_VCSECFeature1 : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_WCUpdateInhibit : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_alarmTriggerRequest : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autoRollWindowsOnLockEnable : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_autopilotPowerStateRequest : 25|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_batteryPreconditioningRequest : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_coastDownMode : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_conditionalLoggingEnabledVCSE : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_efuseMXResistanceEstArmed : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_freeRollModeRequest : 19|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_gloveboxRequest : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_lightSwitch : 9|3@1+ (1,0) [0|4] """"  Receiver
 SG_ UI_locksPanelActive : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_readyToAddKey : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_shorted12VCellTestMode : 27|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_soundHornOnLock : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_summonState : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_trunkRequest : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_userPresent : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_windowRequest : 20|3@1+ (1,0) [0|4] """"  Receiver

BO_ 963 ID3C3VCRIGHT_switchStatus: 7 VehicleBus
 SG_ VCRIGHT_2RowSeatReclineSwitch : 54|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowAutoDownRF : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowAutoDownRR : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowAutoUpRF : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowAutoUpRR : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowDownRF : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowDownRR : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackAutoDwnLF : 35|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackAutoDwnLR : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackAutoDwnRR : 52|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackAutoUpLF : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackAutoUpLR : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackAutoUpRR : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackDownLF : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackDownLR : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackDownRR : 51|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackUpLF : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackUpLR : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowSwPackUpRR : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowUpRF : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_btnWindowUpRR : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_frontBuckleSwitch : 40|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontOccupancySwitch : 42|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatBackrestBack : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatBackrestForward : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatLiftDown : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatLiftUp : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatLumbarDown : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatLumbarIn : 20|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatLumbarOut : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatLumbarUp : 18|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatTiltDown : 4|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatTiltUp : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatTrackBack : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_frontSeatTrackForward : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_liftgateShutfaceSwitchPr : 53|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_rearCenterBuckleSwitch : 44|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_rearRightBuckleSwitch : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_trunkExtReleasePressed : 48|1@1+ (1,0) [0|1] """"  Receiver

BO_ 995 ID3E3VCRIGHT_lightStatus: 2 VehicleBus
 SG_ VCRIGHT_brakeLightStatus : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_interiorTrunkLightStatus : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_rearFogLightStatus : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_reverseLightStatus : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_tailLightStatus : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_turnSignalStatus : 4|2@1+ (1,0) [0|3] """"  Receiver

BO_ 1622 ID656FrontDIinfo: 8 VehicleBus
 SG_ DIF_infoIndex M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_FPGA_version m16 : 48|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_appGitHash m17 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ DIF_applicationCrc m13 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIF_assemblyId m11 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_bootGitHash m18 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ DIF_bootloaderCrc m20 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIF_buildConfigurationId m10 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIF_buildType m10 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DIF_componentId m10 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIF_hardwareId m10 : 32|8@1+ (1,0) [0|252] """"  Receiver
 SG_ DIF_infoBootLdUdsProtocolVersion m20 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_oilPumpAppCrc m16 : 16|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIF_oilPumpBuildType m16 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DIF_pcbaId m11 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_platformTyp m19 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_subUsageId m11 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIF_subcomponentGitHash m31 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ DIF_usageId m11 : 32|16@1+ (1,0) [0|65535] """"  Receiver

BO_ 768 ID300BMS_info: 8 VehicleBus
 SG_ BMS_infoIndex M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_appCrc m13 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ BMS_appGitHash m17 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ BMS_assemblyId m11 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_bootCrc m20 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ BMS_bootGitHash m18 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ BMS_bootUdsProtoVersion m20 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_buildConfigId m10 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ BMS_buildType m10 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ BMS_componentId m10 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ BMS_hardwareId m10 : 32|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ BMS_pcbaId m11 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_platformType m13 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_subUsageId m11 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ BMS_usageId m11 : 32|16@1+ (1,0) [0|65535] """"  Receiver

BO_ 530 ID212BMS_status: 8 VehicleBus
 SG_ BMS_activeHeatingWorthwhile : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_chargeRequest : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_chargeRetryCount : 51|3@1+ (1,0) [0|7] """"  Receiver
 SG_ BMS_chgPowerAvailable : 40|11@1+ (0.125,0) [0|255.75] ""kW""  Receiver
 SG_ BMS_contactorState : 8|3@1+ (1,0) [0|6] """"  Receiver
 SG_ BMS_cpMiaOnHvs : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_diLimpRequest : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_ecuLogUploadRequest : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ BMS_hvState : 16|3@1+ (1,0) [0|6] """"  Receiver
 SG_ BMS_hvacPowerRequest : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_isolationResistance : 19|10@1+ (10,0) [0|10000] ""kOhm""  Receiver
 SG_ BMS_keepWarmRequest : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_notEnoughPowerForDrive : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_notEnoughPowerForSupport : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_okToShipByAir : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_okToShipByLand : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_pcsPwmEnabled : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_preconditionAllowed : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_smStateRequest : 56|4@1+ (1,0) [0|9] """"  Receiver
 SG_ BMS_state : 32|4@1+ (1,0) [0|10] """"  Receiver
 SG_ BMS_uiChargeStatus : 11|3@1+ (1,0) [0|5] """"  Receiver
 SG_ BMS_updateAllowed : 4|1@1+ (1,0) [0|1] """"  Receiver

BO_ 796 ID31CCC_chgStatus: 8 VehicleBus
 SG_ CC_currentLimit : 0|8@1+ (0.5,0) [0|127.5] ""A""  Receiver
 SG_ CC_deltaTransformer : 28|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CC_gridGrounding : 26|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CC_line1Voltage : 16|9@1+ (1,0) [0|511] ""V""  Receiver
 SG_ CC_line2Voltage : 33|9@1+ (1,0) [0|511] ""V""  Receiver
 SG_ CC_line3Voltage : 42|9@1+ (1,0) [0|511] ""V""  Receiver
 SG_ CC_numPhases : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CC_numVehCharging : 30|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CC_pilotState : 8|2@1+ (1,0) [0|3] """"  Receiver

BO_ 573 ID23DCP_chargeStatus: 4 VehicleBus
 SG_ CP_acChargeCurrentLimit : 8|8@1+ (0.5,0) [0|127.5] ""A""  Receiver
 SG_ CP_chargeShutdownRequest : 3|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_hvChargeStatus : 0|3@1+ (1,0) [0|6] """"  Receiver
 SG_ CP_internalMaxCurrentLimit : 16|13@1+ (0.146502,0) [0|1200] ""A""  Receiver
 SG_ CP_vehicleIsoCheckRequired : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_vehiclePrechargeRequired : 30|1@1+ (1,0) [0|1] """"  Receiver

BO_ 317 ID13DCP_chargeStatus: 6 VehicleBus
 SG_ CP_acChargeCurrentLimit : 8|8@1+ (0.5,0) [0|127.5] ""A""  Receiver
 SG_ CP_chargeShutdownRequest : 3|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_evseChargeType : 40|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_hvChargeStatus : 0|3@1+ (1,0) [0|6] """"  Receiver
 SG_ CP_internalMaxAcCurrentLimit : 32|8@1+ (0.5,0) [0|127.5] ""A""  Receiver
 SG_ CP_internalMaxDcCurrentLimit : 16|13@1+ (0.146502,0) [0|1200] ""A""  Receiver
 SG_ CP_vehicleIsoCheckRequired : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_vehiclePrechargeRequired : 30|1@1+ (1,0) [0|1] """"  Receiver

BO_ 1085 ID43DCP_chargeStatusLog: 6 VehicleBus
 SG_ CP_acChargeCurrentLimit_log : 8|8@1+ (0.5,0) [0|127.5] ""A""  Receiver
 SG_ CP_chargeShutdownRequest_log : 3|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_evseChargeType_log : 40|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_hvChargeStatus_log : 0|3@1+ (1,0) [0|6] """"  Receiver
 SG_ CP_internalMaxAcCurrentLimit_log : 32|8@1+ (0.5,0) [0|127.5] ""A""  Receiver
 SG_ CP_internalMaxDcCurrentLimit_log : 16|13@1+ (0.146502,0) [0|1200] ""A""  Receiver
 SG_ CP_vehicleIsoCheckRequired_log : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_vehiclePrechargeRequired_log : 30|1@1+ (1,0) [0|1] """"  Receiver

BO_ 541 ID21DCP_evseStatus: 8 VehicleBus
 SG_ CP_acChargeState : 53|3@1+ (1,0) [0|6] """"  Receiver
 SG_ CP_acNumRetries : 40|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_cableCurrentLimit : 24|7@1+ (1,0) [0|127] ""A""  Receiver
 SG_ CP_cableType : 16|3@1+ (1,0) [0|4] """"  Receiver
 SG_ CP_digitalCommsAttempts : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_digitalCommsEstablished : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_evseAccept : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_evseChargeType_UI : 38|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_evseRequest : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_gbState : 42|4@1+ (1,0) [0|15] """"  Receiver
 SG_ CP_gbdcChargeAttempts : 51|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_gbdcFailureReason : 49|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_gbdcStopChargeReason : 46|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CP_iecComboState : 60|4@1+ (1,0) [0|12] """"  Receiver
 SG_ CP_pilot : 4|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CP_pilotCurrent : 8|8@1+ (0.5,0) [0|127.5] ""A""  Receiver
 SG_ CP_proximity : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ CP_teslaDcState : 56|4@1+ (1,0) [0|10] """"  Receiver
 SG_ CP_teslaSwcanState : 34|3@1+ (1,0) [0|6] """"  Receiver

BO_ 1859 ID743VCRIGHT_recallStatus: 1 VehicleBus
 SG_ VCRIGHT_mirrorRecallStatus : 4|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_seatRecallStatus : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_systemRecallStatus : 0|2@1+ (1,0) [0|3] """"  Receiver

BO_ 1885 ID75DCP_sensorData: 8 VehicleBus
 SG_ CP_sensorDataSelect M : 0|4@1+ (1,0) [0|12] """"  Receiver
 SG_ CP_UHF_chipState m6 : 4|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CP_UHF_fifoData m6 : 32|8@1+ (1,0) [0|255] """"  Receiver
 SG_ CP_UHF_rssi m6 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ CP_UHF_rxNumBytes m6 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ CP_UHF_rxOverflow m6 : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_UHF_selfTestRssi m6 : 40|8@1+ (1,0) [0|255] """"  Receiver
 SG_ CP_backCover2Counts m5 : 32|16@1+ (0.0015259,0) [0|100] ""%""  Receiver
 SG_ CP_backCoverCounts m5 : 16|16@1+ (0.0015259,0) [0|100] ""%""  Receiver
 SG_ CP_boardTemperature m1 : 32|8@1+ (1.29412,-50) [-50|280] ""C""  Receiver
 SG_ CP_doorCountsDebounced m0 : 24|16@1+ (0.0015259,0) [0|100] ""%""  Receiver
 SG_ CP_doorCountsFiltered m0 : 8|16@1+ (0.0015259,0) [0|100] ""%""  Receiver
 SG_ CP_doorI m2 : 16|12@1+ (0.0025,0) [0|10.2375] ""A""  Receiver
 SG_ CP_doorLastRequestMaxI m2 : 28|12@1+ (0.0025,0) [0|10.2375] ""A""  Receiver
 SG_ CP_doorPot m2 : 4|12@1+ (0.025,0) [0|100] ""%""  Receiver
 SG_ CP_faultLineV m5 : 4|12@1+ (0.00114,0) [0|4.65524] ""V""  Receiver
 SG_ CP_inductiveSensor_raw m4 : 4|28@1+ (1,0) [0|268435000] """"  Receiver
 SG_ CP_inlet1HarnessIdState m12 : 4|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_inlet1HarnessIdValue m12 : 8|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CP_inlet1HarnessV m12 : 16|14@1+ (0.001,0) [0|13.3] ""V""  Receiver
 SG_ CP_inlet2HarnessIdState m12 : 30|2@1+ (1,0) [0|2] """"  Receiver
 SG_ CP_inlet2HarnessIdValue m12 : 32|3@1+ (1,0) [0|7] """"  Receiver
 SG_ CP_inlet2HarnessV m12 : 40|14@1+ (0.001,0) [0|13.3] ""V""  Receiver
 SG_ CP_latch2I m3 : 16|12@1+ (0.0025,0) [0|10.2375] ""A""  Receiver
 SG_ CP_latchI m3 : 4|12@1+ (0.0025,0) [0|10.2375] ""A""  Receiver
 SG_ CP_pilotHighValue m9 : 40|12@1- (0.01,0) [-20.48|20.47] ""V""  Receiver
 SG_ CP_pilotHighValue_intervalMax10s m10 : 28|12@1- (0.01,0) [-20.48|20.47] ""V""  Receiver
 SG_ CP_pilotHighValue_intervalMin10s m10 : 40|12@1- (0.01,0) [-20.48|20.47] ""V""  Receiver
 SG_ CP_pilotLowValue m9 : 28|12@1- (0.01,0) [-20.48|20.47] ""V""  Receiver
 SG_ CP_pilotLowValue_intervalMax10s m10 : 4|12@1- (0.01,0) [-20.48|20.47] ""V""  Receiver
 SG_ CP_pilotLowValue_intervalMin10s m10 : 16|12@1- (0.01,0) [-20.48|20.47] ""V""  Receiver
 SG_ CP_pilotPeriod m9 : 16|12@1+ (1,0) [0|4095] """"  Receiver
 SG_ CP_pilotPulseWidth m9 : 4|12@1+ (1,0) [0|4095] """"  Receiver
 SG_ CP_pinTemperature1 m1 : 8|8@1+ (0.803922,-55) [-55|149.99] ""C""  Receiver
 SG_ CP_pinTemperature2 m1 : 16|8@1+ (0.803922,-55) [-55|149.99] ""C""  Receiver
 SG_ CP_pinTemperature3 m1 : 24|8@1+ (0.803922,-55) [-55|149.99] ""C""  Receiver
 SG_ CP_proxConn1Sense m8 : 56|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_proxEn m8 : 57|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CP_proximityV m8 : 8|16@1+ (0.0001,0) [0|6.5535] ""V""  Receiver
 SG_ CP_proximityV_GBCC1 m11 : 8|14@1+ (0.001,0) [0|13.3] ""V""  Receiver
 SG_ CP_proximityV_GBCC2 m11 : 24|14@1+ (0.001,0) [0|13.3] ""V""  Receiver
 SG_ CP_proximityV_intervalMax10s m8 : 40|16@1+ (0.0001,0) [0|6.5535] ""V""  Receiver
 SG_ CP_proximityV_intervalMin10s m8 : 24|16@1+ (0.0001,0) [0|6.5535] ""V""  Receiver
 SG_ CP_refVoltage m7 : 8|8@1+ (0.00705882,0) [0|1.8] ""V""  Receiver

BO_ 647 ID287PTCcabinHeatSensorStatus: 8 VehicleBus
 SG_ PTC_leftCurrentHV : 48|8@1+ (0.2,0) [0|50] ""A""  Receiver
 SG_ PTC_leftTempIGBT : 0|8@1+ (1,-40) [-40|200] ""C""  Receiver
 SG_ PTC_rightCurrentHV : 56|8@1+ (0.2,0) [0|50] ""A""  Receiver
 SG_ PTC_rightTempIGBT : 16|8@1+ (1,-40) [-40|200] ""C""  Receiver
 SG_ PTC_tempOCP : 8|8@1+ (1,-40) [-40|200] ""C""  Receiver
 SG_ PTC_tempPCB : 24|8@1+ (1,-40) [-40|200] ""C""  Receiver
 SG_ PTC_voltageHV : 32|10@1+ (0.5,0) [0|511.5] ""V""  Receiver

BO_ 819 ID333UI_chargeRequest: 5 VehicleBus
 SG_ UI_acChargeCurrentLimit : 8|7@1+ (1,0) [0|127] ""A""  Receiver
 SG_ UI_brickBalancingDisabled : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_brickVLoggingRequest : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_chargeEnableRequest : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_chargeTerminationPct : 16|10@1+ (0.1,0) [25|100] ""%""  Receiver
 SG_ UI_closeChargePortDoorRequest : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_openChargePortDoorRequest : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_scheduledDepartureEnabled : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_smartAcChargingEnabled : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_cpInletHeaterRequest : 32|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UI_socSnapshotExpirationTime : 28|4@1+ (1,2) [0|0] ""weeks""  Receiver

BO_ 820 ID334UI_powertrainControl: 8 VehicleBus
 SG_ UI_DIAppSliderDebug : 42|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_limitMode : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_motorOnMode : 34|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_pedalMap : 5|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_powertrainControlChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ UI_powertrainControlCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_regenTorqueMax : 24|8@1+ (0.5,0) [0|100] ""%""  Receiver
 SG_ UI_speedLimit : 16|8@1+ (1,50) [50|285] ""kph""  Receiver
 SG_ UI_stoppingMode : 40|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_systemPowerLimit : 0|5@1+ (20,20) [20|640] ""kW""  Receiver
 SG_ UI_systemTorqueLimit : 8|6@1+ (100,4000) [4000|10300] ""Nm""  Receiver
 SG_ UI_wasteMode : 36|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_wasteModeRegenLimit : 38|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_closureConfirmed : 14|2@1+ (1,0) [0|2] """"  Receiver

BO_ 826 ID33AUI_rangeSOC: 8 VehicleBus
 SG_ UI_idealRange : 16|10@1+ (1,0) [0|1023] ""mi""  Receiver
 SG_ UI_Range : 0|10@1+ (1,0) [0|1023] ""mi""  Receiver
 SG_ UI_SOC : 48|7@1+ (1,0) [0|127] ""%""  Receiver
 SG_ UI_uSOE : 56|7@1+ (1,0) [0|127] ""%""  Receiver
 SG_ UI_ratedWHpM : 32|10@1+ (1,0) [0|1023] ""WHpM""  Receiver

BO_ 577 ID241VCFRONT_coolant: 7 VehicleBus
 SG_ VCFRONT_coolantAirPurgeBatState : 48|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_coolantFlowBatActual : 0|9@1+ (0.1,0) [0|40] ""LPM""  Receiver
 SG_ VCFRONT_coolantFlowBatReason : 18|4@1+ (1,0) [0|14] """"  Receiver
 SG_ VCFRONT_coolantFlowBatTarget : 9|9@1+ (0.1,0) [0|40] ""LPM""  Receiver
 SG_ VCFRONT_coolantFlowPTActual : 22|9@1+ (0.1,0) [0|40] ""LPM""  Receiver
 SG_ VCFRONT_coolantFlowPTReason : 40|4@1+ (1,0) [0|14] """"  Receiver
 SG_ VCFRONT_coolantFlowPTTarget : 31|9@1+ (0.1,0) [0|40] ""LPM""  Receiver
 SG_ VCFRONT_coolantHasBeenFilled : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_radiatorIneffective : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_wasteHeatRequestType : 44|2@1+ (1,0) [0|2] """"  Receiver

BO_ 955 ID3BBUI_power: 2 VehicleBus
 SG_ UI_powerExpected : 0|8@1+ (1,0) [0|100] ""kW""  Receiver
 SG_ UI_powerIdeal : 8|8@1+ (1,0) [0|100] ""kW""  Receiver

BO_ 1493 ID5D5RearDItemps: 5 VehicleBus
 SG_ DI_ph1Temp : 0|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DI_ph2Temp : 8|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DI_ph3Temp : 16|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DI_pcbTemp2 : 24|8@1+ (1,-40) [-40|150] ""C""  Receiver
 SG_ DI_IGBTJunctTemp : 32|8@1+ (1,-40) [-40|200] ""C""  Receiver

BO_ 1366 ID556FrontDItemps: 7 VehicleBus
 SG_ DIF_ph1Temp : 0|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DIF_ph2Temp : 8|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DIF_ph3Temp : 16|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DIF_pcbTemp2 : 24|8@1+ (1,-40) [-40|150] ""C""  Receiver
 SG_ DIF_IGBTJunctTemp : 32|8@1+ (1,-40) [-40|200] ""C""  Receiver
 SG_ DIF_lashAngle : 40|10@1+ (0.06,0) [0|60] ""Deg""  Receiver
 SG_ DIF_lashCheckCount : 50|6@1+ (1,0) [0|63] """"  Receiver

BO_ 1367 ID557FrontThermalControl: 4 VehicleBus
 SG_ DIS_activeInletTempReq : 8|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DIS_coolantFlowReq : 16|8@1+ (0.2,0) [0|50] ""LPM""  Receiver
 SG_ DIS_oilFlowReq : 24|8@1+ (0.06,0) [0|15] ""LPM""  Receiver
 SG_ DIS_passiveInletTempReq : 0|8@1+ (1,-40) [-40|120] ""C""  Receiver

BO_ 1495 ID5D7RearThermalControl: 4 VehicleBus
 SG_ DI_activeInletTempReq : 8|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ DI_coolantFlowReq : 16|8@1+ (0.2,0) [0|50] ""LPM""  Receiver
 SG_ DI_oilFlowReq : 24|8@1+ (0.06,0) [0|15] ""LPM""  Receiver
 SG_ DI_passiveInletTempReq : 0|8@1+ (1,-40) [-40|120] ""C""  Receiver

BO_ 2005 ID7D5DIR_debug: 8 VehicleBus
 SG_ DIR_debugSelector M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_brakeSwitchNC m42 : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_brakeSwitchNO m42 : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_busbarTemp m64 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_controlStack m67 : 40|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_cpu100HzAvg m66 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu100HzMin m66 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu10HzAvg m66 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu10HzMin m66 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu10msMin m66 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu1HzAvg m128 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu1HzMin m128 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu1kHzAvg m67 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu1kHzMin m67 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu20kHzAvg m67 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpu20kHzMin m67 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_cpuIDWord0 m131 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIR_cpuIDWord1 m132 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIR_cpuIDWord2 m132 : 32|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIR_cpuIDWord3 m132 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIR_crc m131 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIR_currentLimit m41 : 32|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIR_dcCableCurrentEst m37 : 16|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_dcCableHeat m47 : 32|16@1+ (0.001,0) [0|65.535] ""kA2s""  Receiver
 SG_ DIR_dcCapTemp m64 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_dcLinkCapTemp m70 : 32|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_decodeHardwareStack m68 : 24|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_driveUnitOdometer m69 : 32|32@1+ (10,0) [0|42949700000] ""rev""  Receiver
 SG_ DIR_eepromStack m68 : 32|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_fluxState m42 : 16|4@1+ (1,0) [0|10] """"  Receiver
 SG_ DIR_gainScale m37 : 8|8@1+ (0.01,0) [0|2.55] ""scale""  Receiver
 SG_ DIR_gateDriveState m50 : 10|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DIR_gateDriveSupplyState m50 : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DIR_hvDcCableTemp m70 : 40|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_hwFaultCount m69 : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DIR_idleStack m68 : 16|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_immobilizerStack m68 : 56|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_internalAngleFilt m37 : 48|16@1+ (0.0003,0) [0|8] ""rad""  Receiver
 SG_ DIR_llrScale m42 : 32|8@1+ (0.015,0) [0|3.825] ""scale""  Receiver
 SG_ DIR_llsScale m42 : 24|8@1+ (0.004,0) [0|1.02] ""scale""  Receiver
 SG_ DIR_lmScale m36 : 8|8@1+ (0.004,0) [0|1.02] ""scale""  Receiver
 SG_ DIR_loadAngle m42 : 40|16@1- (0.0003,0) [-4|4] ""rad""  Receiver
 SG_ DIR_loadAngleMargin m37 : 32|16@1- (0.0003,0) [-4|4] ""rad""  Receiver
 SG_ DIR_magnetTempEst m47 : 48|8@1+ (1,-40) [-40|180] ""C""  Receiver
 SG_ DIR_module10HzStack m66 : 48|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_motorIA m32 : 16|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIR_motorIAavg m38 : 32|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_motorIB m32 : 32|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIR_motorIBavg m38 : 48|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_motorIC m32 : 48|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIR_motorType m128 : 8|6@1+ (1,0) [0|32] """"  Receiver
 SG_ DIR_motorV m34 : 48|16@1+ (2E-005,0) [0|1.3107] ""mindex""  Receiver
 SG_ DIR_negDcBusbarTemp m70 : 48|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_offsetA m32 : 8|8@1- (0.1,0) [-12.8|12.7] ""A""  Receiver
 SG_ DIR_offsetB m33 : 8|8@1- (0.1,0) [-12.8|12.7] ""A""  Receiver
 SG_ DIR_oilPumpMotorSpeed m46 : 8|8@1+ (40,0) [0|10200] ""RPM""  Receiver
 SG_ DIR_oilPumpPhaseVoltage m46 : 16|8@1+ (0.1,0) [0|25.4] ""V""  Receiver
 SG_ DIR_oilPumpPressureEstimateMax m46 : 24|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ DIR_oilPumpPressureExpectedMin m46 : 32|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ DIR_pcsTemp m64 : 24|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_peakFlux m36 : 48|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIR_peakIQref m38 : 16|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIR_phaseOutBusbarTemp m70 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_phaseOutBusbarWeldTemp m70 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_phaseOutLugTemp m70 : 24|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_posDcBusbarTemp m70 : 56|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_powerStageSafeState m50 : 13|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DIR_pwmState m63 : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DIR_pwrSatChargeCurrent m42 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_pwrSatDischargeCurrent m40 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_pwrSatMaxBusVoltage m39 : 48|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_pwrSatMaxDischargePower m39 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_pwrSatMaxRegenPower m39 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_pwrSatMinBusVoltage m39 : 56|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_resolverClaMIA m48 : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_resolverCommonGain m48 : 32|8@1+ (0.025,0) [0|5] ""1""  Receiver
 SG_ DIR_resolverCosFiltered m52 : 32|8@1- (0.01,0) [-1|1] ""1""  Receiver
 SG_ DIR_resolverCosRmsSquared m52 : 16|8@1+ (0.005,0) [0|1] ""1""  Receiver
 SG_ DIR_resolverErrorRmsSquared m52 : 48|8@1+ (0.005,0) [0|1] ""1""  Receiver
 SG_ DIR_resolverNoCarrier m48 : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_resolverNoPhaseLock m48 : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_resolverOffsetCos m48 : 8|8@1+ (0.015,0) [0|3.3] ""V""  Receiver
 SG_ DIR_resolverOffsetSin m48 : 16|8@1+ (0.015,0) [0|3.3] ""V""  Receiver
 SG_ DIR_resolverPhaseOffset m48 : 24|8@1+ (0.1,7.5) [7.5|32.5] ""us""  Receiver
 SG_ DIR_resolverReady m48 : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_resolverSinFiltered m52 : 40|8@1- (0.01,0) [-1|1] ""1""  Receiver
 SG_ DIR_resolverSinRmsSquared m52 : 24|8@1+ (0.005,0) [0|1] ""1""  Receiver
 SG_ DIR_rotorFlux m47 : 16|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIR_rotorMaxMagnetTemp m72 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_rotorOffsetEst m43 : 8|12@1- (0.01,0) [-20|20] ""deg""  Receiver
 SG_ DIR_rotorOffsetLearningState m63 : 8|4@1+ (1,0) [0|9] """"  Receiver
 SG_ DIR_rotorOffsetMean m43 : 20|12@1- (0.01,0) [-20|20] ""deg""  Receiver
 SG_ DIR_rsScale m34 : 8|8@1+ (0.01,0) [0|2.55] ""scale""  Receiver
 SG_ DIR_soptMaxCurrentMagSqrd m49 : 40|16@1+ (100,0) [0|6553500] ""A2""  Receiver
 SG_ DIR_soptTimeToOff m49 : 24|11@1+ (0.05,0) [0|100] ""ms""  Receiver
 SG_ DIR_soptTimeToTrip m49 : 8|8@1+ (0.05,0) [0|10] ""ms""  Receiver
 SG_ DIR_soptTripDelay m49 : 16|8@1+ (0.05,0) [0|10] ""ms""  Receiver
 SG_ DIR_ssmState m38 : 8|4@1+ (1,0) [0|8] """"  Receiver
 SG_ DIR_statorEndWindingTemp m72 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_statorFluxFdb m35 : 32|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIR_statorFluxRef m35 : 16|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIR_statorIDfdb m33 : 32|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_statorIDref m33 : 16|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_statorIQfdb m34 : 32|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_statorIQref m34 : 16|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_statorTemp1 m64 : 32|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_statorTemp2 m64 : 40|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_statorVD m36 : 32|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIR_statorVDFiltered m63 : 16|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIR_statorVQ m36 : 16|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIR_statorVQFiltered m63 : 32|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIR_sysHeatPowerOptimal m50 : 16|8@1+ (0.08,0) [0|20] ""kW""  Receiver
 SG_ DIR_systemStack m68 : 8|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_systemTorqueCommand m35 : 48|16@1- (0.035,0) [-1146.88|1146.85] ""Nm""  Receiver
 SG_ DIR_tcMaxRequest m40 : 16|8@1+ (5,0) [0|1275] ""Nm""  Receiver
 SG_ DIR_tcMinRequest m40 : 24|8@1+ (5,0) [0|1275] ""Nm""  Receiver
 SG_ DIR_torquePerAmp m33 : 48|16@1+ (0.0001,0) [0|6.5535] ""Nm/A""  Receiver
 SG_ DIR_tqSatMotorCurrent m39 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_tqSatMotorVoltage m39 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_tqSatThermal m39 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_tqSatUiDriveTorque m40 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_tqSatUiRegenTorque m40 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_tqScaleDifferential m35 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_tqScaleMaxMotorSpeed m40 : 48|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_tqScaleShift m40 : 56|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIR_udsStack m68 : 40|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_usmState m38 : 12|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DIR_veMassInvRaw m41 : 8|12@1+ (1E-007,0.0001) [0.0001|0.0005] ""1/kg""  Receiver
 SG_ DIR_veResForce m41 : 20|12@1- (0.0005,0) [-1.024|1.0235] ""G""  Receiver
 SG_ DIR_wasteCurrentLimit m41 : 48|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIR_xcpStack m68 : 48|8@1+ (0.4,0) [0|100] ""%""  Receiver

BO_ 1879 ID757DIF_debug: 8 VehicleBus
 SG_ DIF_debugSelector M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_brakeSwitchNC m42 : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIF_brakeSwitchNO m42 : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIF_busbarTemp m64 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_controlStack m67 : 40|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_cpu100HzAvg m66 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu100HzMin m66 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu10HzAvg m66 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu10HzMin m66 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu10msMin m66 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu1HzAvg m128 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu1HzMin m128 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu1kHzAvg m67 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu1kHzMin m67 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu20kHzAvg m67 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpu20kHzMin m67 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_cpuIDWord0 m131 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIF_cpuIDWord1 m132 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIF_cpuIDWord2 m132 : 32|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIF_cpuIDWord3 m132 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ DIF_crc m131 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ DIF_currentLimit m41 : 32|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIF_dcCableCurrentEst m37 : 16|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_dcCableHeat m47 : 32|16@1+ (0.001,0) [0|65.535] ""kA2s""  Receiver
 SG_ DIF_dcCapTemp m64 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_dcLinkCapTemp m70 : 32|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_decodeHardwareStack m68 : 24|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_driveUnitOdometer m69 : 32|32@1+ (10,0) [0|42949700000] ""rev""  Receiver
 SG_ DIF_eepromStack m68 : 32|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_fluxState m42 : 16|4@1+ (1,0) [0|10] """"  Receiver
 SG_ DIF_gainScale m37 : 8|8@1+ (0.01,0) [0|2.55] ""scale""  Receiver
 SG_ DIF_gateDriveState m50 : 10|3@1+ (1,0) [0|4] """"  Receiver
 SG_ DIF_gateDriveSupplyState m50 : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DIF_hvDcCableTemp m70 : 40|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_hwFaultCount m69 : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DIF_idleStack m68 : 16|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_immobilizerStack m68 : 56|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_internalAngleFilt m37 : 48|16@1+ (0.0003,0) [0|8] ""rad""  Receiver
 SG_ DIF_llrScale m42 : 32|8@1+ (0.015,0) [0|3.825] ""scale""  Receiver
 SG_ DIF_llsScale m42 : 24|8@1+ (0.004,0) [0|1.02] ""scale""  Receiver
 SG_ DIF_lmScale m36 : 8|8@1+ (0.004,0) [0|1.02] ""scale""  Receiver
 SG_ DIF_loadAngle m42 : 40|16@1- (0.0003,0) [-4|4] ""rad""  Receiver
 SG_ DIF_loadAngleMargin m37 : 32|16@1- (0.0003,0) [-4|4] ""rad""  Receiver
 SG_ DIF_magnetTempEst m47 : 48|8@1+ (1,-40) [-40|180] ""C""  Receiver
 SG_ DIF_module10HzStack m66 : 48|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_motorIA m32 : 16|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIF_motorIAavg m38 : 32|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_motorIB m32 : 32|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIF_motorIBavg m38 : 48|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_motorIC m32 : 48|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIF_motorType m128 : 8|6@1+ (1,0) [0|32] """"  Receiver
 SG_ DIF_motorV m34 : 48|16@1+ (2E-005,0) [0|1.3107] ""mindex""  Receiver
 SG_ DIF_negDcBusbarTemp m70 : 48|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_offsetA m32 : 8|8@1- (0.1,0) [-12.8|12.7] ""A""  Receiver
 SG_ DIF_offsetB m33 : 8|8@1- (0.1,0) [-12.8|12.7] ""A""  Receiver
 SG_ DIF_oilPumpMotorSpeed m46 : 8|8@1+ (40,0) [0|10200] ""RPM""  Receiver
 SG_ DIF_oilPumpPhaseVoltage m46 : 16|8@1+ (0.1,0) [0|25.4] ""V""  Receiver
 SG_ DIF_oilPumpPressureEstimateMax m46 : 24|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ DIF_oilPumpPressureExpectedMin m46 : 32|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ DIF_pcsTemp m64 : 24|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_peakFlux m36 : 48|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIF_peakIQref m38 : 16|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIF_phaseOutBusbarTemp m70 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_phaseOutBusbarWeldTemp m70 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_phaseOutLugTemp m70 : 24|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_posDcBusbarTemp m70 : 56|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_powerStageSafeState m50 : 13|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DIF_pwmState m63 : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DIF_pwrSatChargeCurrent m42 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_pwrSatDischargeCurrent m40 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_pwrSatMaxBusVoltage m39 : 48|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_pwrSatMaxDischargePower m39 : 16|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_pwrSatMaxRegenPower m39 : 24|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_pwrSatMinBusVoltage m39 : 56|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_resolverClaMIA m48 : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIF_resolverCommonGain m48 : 32|8@1+ (0.025,0) [0|5] ""1""  Receiver
 SG_ DIF_resolverCosFiltered m52 : 32|8@1- (0.01,0) [-1|1] ""1""  Receiver
 SG_ DIF_resolverCosRmsSquared m52 : 16|8@1+ (0.005,0) [0|1] ""1""  Receiver
 SG_ DIF_resolverErrorRmsSquared m52 : 48|8@1+ (0.005,0) [0|1] ""1""  Receiver
 SG_ DIF_resolverNoCarrier m48 : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIF_resolverNoPhaseLock m48 : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIF_resolverOffsetCos m48 : 8|8@1+ (0.015,0) [0|3.3] ""V""  Receiver
 SG_ DIF_resolverOffsetSin m48 : 16|8@1+ (0.015,0) [0|3.3] ""V""  Receiver
 SG_ DIF_resolverPhaseOffset m48 : 24|8@1+ (0.1,7.5) [7.5|32.5] ""us""  Receiver
 SG_ DIF_resolverReady m48 : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIF_resolverSinFiltered m52 : 40|8@1- (0.01,0) [-1|1] ""1""  Receiver
 SG_ DIF_resolverSinRmsSquared m52 : 24|8@1+ (0.005,0) [0|1] ""1""  Receiver
 SG_ DIF_rotorFlux m47 : 16|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIF_rotorMaxMagnetTemp m72 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_rotorOffsetEst m43 : 8|12@1- (0.01,0) [-20|20] ""deg""  Receiver
 SG_ DIF_rotorOffsetLearningState m63 : 8|4@1+ (1,0) [0|9] """"  Receiver
 SG_ DIF_rotorOffsetMean m43 : 20|12@1- (0.01,0) [-20|20] ""deg""  Receiver
 SG_ DIF_rsScale m34 : 8|8@1+ (0.01,0) [0|2.55] ""scale""  Receiver
 SG_ DIF_soptMaxCurrentMagSqrd m49 : 40|16@1+ (100,0) [0|6553500] ""A2""  Receiver
 SG_ DIF_soptTimeToOff m49 : 24|11@1+ (0.05,0) [0|100] ""ms""  Receiver
 SG_ DIF_soptTimeToTrip m49 : 8|8@1+ (0.05,0) [0|10] ""ms""  Receiver
 SG_ DIF_soptTripDelay m49 : 16|8@1+ (0.05,0) [0|10] ""ms""  Receiver
 SG_ DIF_ssmState m38 : 8|4@1+ (1,0) [0|8] """"  Receiver
 SG_ DIF_statorEndWindingTemp m72 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_statorFluxFdb m35 : 32|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIF_statorFluxRef m35 : 16|16@1+ (0.0001,0) [0|6.5535] ""Wb""  Receiver
 SG_ DIF_statorIDfdb m33 : 32|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_statorIDref m33 : 16|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_statorIQfdb m34 : 32|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_statorIQref m34 : 16|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_statorTemp1 m64 : 32|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_statorTemp2 m64 : 40|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_statorVD m36 : 32|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIF_statorVDFiltered m63 : 16|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIF_statorVQ m36 : 16|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIF_statorVQFiltered m63 : 32|16@1- (4E-005,0) [-1.31072|1.31068] ""mindex""  Receiver
 SG_ DIF_sysHeatPowerOptimal m50 : 16|8@1+ (0.08,0) [0|20] ""kW""  Receiver
 SG_ DIF_systemStack m68 : 8|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_systemTorqueCommand m35 : 48|16@1- (0.035,0) [-1146.88|1146.85] ""Nm""  Receiver
 SG_ DIF_tcMaxRequest m40 : 16|8@1+ (5,0) [0|1275] ""Nm""  Receiver
 SG_ DIF_tcMinRequest m40 : 24|8@1+ (5,0) [0|1275] ""Nm""  Receiver
 SG_ DIF_torquePerAmp m33 : 48|16@1+ (0.0001,0) [0|6.5535] ""Nm/A""  Receiver
 SG_ DIF_tqSatMotorCurrent m39 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_tqSatMotorVoltage m39 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_tqSatThermal m39 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_tqSatUiDriveTorque m40 : 32|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_tqSatUiRegenTorque m40 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_tqScaleDifferential m35 : 8|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_tqScaleMaxMotorSpeed m40 : 48|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_tqScaleShift m40 : 56|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ DIF_udsStack m68 : 40|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_usmState m38 : 12|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DIF_veMassInvRaw m41 : 8|12@1+ (1E-007,0.0001) [0.0001|0.0005] ""1/kg""  Receiver
 SG_ DIF_veResForce m41 : 20|12@1- (0.0005,0) [-1.024|1.0235] ""G""  Receiver
 SG_ DIF_wasteCurrentLimit m41 : 48|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ DIF_xcpStack m68 : 48|8@1+ (0.4,0) [0|100] ""%""  Receiver

BO_ 694 ID2B6DI_chassisControlStatus: 2 VehicleBus
 SG_ DI_btcStateUI : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_ptcStateUI : 7|2@1+ (1,0) [0|3] """"  Receiver
 SG_ DI_tcTelltaleFlash : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_tcTelltaleOn : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_tractionControlModeUI : 4|3@1+ (1,0) [0|6] """"  Receiver
 SG_ DI_vdcTelltaleFlash : 0|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_vdcTelltaleOn : 1|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_vehicleHoldTelltaleOn : 10|1@1+ (1,0) [0|1] """"  Receiver

BO_ 644 ID284UIvehicleModes: 5 VehicleBus
 SG_ UIfactoryMode284 : 0|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UIhomelinkV2Command0284 : 8|8@1+ (1,0) [0|0] """"  Receiver
 SG_ UIhomelinkV2Command1284 : 16|8@1+ (1,0) [0|0] """"  Receiver
 SG_ UIhomelinkV2Command2284 : 24|8@1+ (1,0) [0|0] """"  Receiver
 SG_ UIserviceMode284 : 3|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UIshowroomMode284 : 2|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UItransportMode284 : 1|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UIgameMode284 : 34|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UIisDelivered284 : 4|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UIcarWashModeRequest284 : 32|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UIvaletMode284 : 33|1@1+ (1,0) [0|0] """"  Receiver
 SG_ UIsentryMode284 : 5|1@1+ (1,0) [0|0] """"  Receiver

BO_ 545 ID221VCFRONT_LVPowerState: 8 VehicleBus
 SG_ VCFRONT_LVPowerStateIndex M : 0|5@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_CMPDLVState m1 : 18|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_amplifierLVRequest m0 : 28|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_cpLVRequest m1 : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_das1HighCurrentLVState m0 : 30|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_das2HighCurrentLVState m0 : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_difLVState m0 : 36|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_dirLVRequest m0 : 34|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_epasLVState m1 : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_espLVState m0 : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_hvacCompLVState m0 : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_hvcLVRequest m1 : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_iBoosterLVState m0 : 24|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_ocsLVRequest m0 : 42|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_oilPumpFrontLVState m0 : 38|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_oilPumpRearLVRequest m0 : 40|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_parkLVState m0 : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_pcsLVState m1 : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_ptcLVRequest m0 : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_radcLVState m0 : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_rcmLVRequest m0 : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_sccmLVRequest m0 : 18|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_tasLVState m1 : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_tpmsLVRequest m0 : 20|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_tunerLVRequest m0 : 26|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_uiHiCurrentLVState m0 : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_vcleftHiCurrentLVState m0 : 44|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_vcrightHiCurrentLVState m0 : 46|2@1+ (1,0) [0|3] """"  Receiver

BO_ 549 ID225VCRIGHT_LVPowerState: 3 VehicleBus
 SG_ VCRIGHT_amplifierLVState : 4|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_cntctrPwrState : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_eFuseLockoutStatus : 18|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCRIGHT_hvcLVState : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_lumbarLVState : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_ocsLVState : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_ptcLVState : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_rcmLVState : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_rearOilPumpLVState : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_swEnStatus : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_tunerLVState : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_vehiclePowerStateDBG : 21|2@1+ (1,0) [0|3] """"  Receiver

BO_ 753 ID2F1VCFRONT_eFuseDebugStatus: 8 VehicleBus
 SG_ VCFRONT_eFuseDebugStatusIndex M : 0|5@1+ (1,0) [0|18] """"  Receiver
 SG_ VCFRONT_ChargePumpVoltage m16 : 24|16@1+ (0.00544368,0) [0|356.751] ""V""  Receiver
 SG_ VCFRONT_EPAS3PCurrent m4 : 47|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_EPAS3PFault m4 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_EPAS3PSelfTestResult m4 : 11|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCFRONT_EPAS3PState m4 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_EPAS3PTemp m4 : 15|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_EPAS3PVoltage m4 : 31|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_EPAS3SCurrent m5 : 47|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_EPAS3SFault m5 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_EPAS3SSelfTestResult m5 : 11|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCFRONT_EPAS3SState m5 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_EPAS3STemp m5 : 15|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_EPAS3SVoltage m5 : 31|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_ESPMotorCurrent m6 : 47|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_ESPMotorFault m6 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_ESPMotorSelfTestResult m6 : 11|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCFRONT_ESPMotorState m6 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_ESPMotorTemp m6 : 15|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_ESPMotorVoltage m6 : 31|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_ESPValveCurrent m7 : 44|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_ESPValveFault m7 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_ESPValveState m7 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_ESPValveTemp m7 : 12|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_ESPValveVoltage m7 : 28|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_IBSUnfilteredTemperature m17 : 40|16@1- (0.01,0) [-327.68|327.67] ""C""  Receiver
 SG_ VCFRONT_PCSCurrent m2 : 47|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ VCFRONT_PCSFault m2 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_PCSState m2 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_PCSTemp m2 : 15|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_PCSVoltage m2 : 31|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_autopilot1Current m8 : 43|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_autopilot1Fault m8 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_autopilot1State m8 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_autopilot1Temp m8 : 11|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_autopilot1Voltage m8 : 27|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_autopilot2Current m9 : 43|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_autopilot2Fault m9 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_autopilot2State m9 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_autopilot2Temp m9 : 11|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_autopilot2Voltage m9 : 27|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_chargedIBSAmpHours m17 : 8|16@1+ (1,0) [0|65535] ""Ah""  Receiver
 SG_ VCFRONT_dischargedIBSAmpHours m17 : 24|16@1+ (1,0) [0|65535] ""Ah""  Receiver
 SG_ VCFRONT_eFuseLockoutVoltage m16 : 8|12@1+ (0.00544368,0) [0|22.2919] ""V""  Receiver
 SG_ VCFRONT_headlampLeftCurrent m12 : 10|10@1+ (0.1,0) [0|102.3] ""A""  Receiver
 SG_ VCFRONT_headlampLeftFault m12 : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_headlampLeftState m12 : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_headlampLeftTemperature m12 : 48|8@1- (1,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_headlampLeftVoltage m12 : 20|8@1+ (0.1,0) [0|25.4] ""V""  Receiver
 SG_ VCFRONT_headlampRightCurrent m12 : 30|10@1+ (0.1,0) [0|102.3] ""A""  Receiver
 SG_ VCFRONT_headlampRightFault m12 : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_headlampRightState m12 : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_headlampRightTemperature m12 : 56|8@1- (1,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_headlampRightVoltage m12 : 40|8@1+ (0.1,0) [0|25.4] ""V""  Receiver
 SG_ VCFRONT_iBoosterCurrent m3 : 47|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_iBoosterFault m3 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_iBoosterSelfTestResult m3 : 11|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCFRONT_iBoosterState m3 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_iBoosterTemp m3 : 15|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_iBoosterVoltage m3 : 31|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_leftControllerCurrent m1 : 47|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_leftControllerFault m1 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_leftControllerState m1 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_leftControllerTemp m1 : 15|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_leftControllerVoltage m1 : 31|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_pcsSelfTestResult m2 : 11|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCFRONT_pump1AndFanCurrent m14 : 6|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_pump1AndFanState m14 : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_pump1AndFanVoltage m14 : 22|12@1+ (0.1,0) [0|409.5] ""V""  Receiver
 SG_ VCFRONT_pump2AndAirCompCurrent m14 : 35|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_pump2AndAirCompState m14 : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_pump2AndAirCompVoltage m14 : 51|12@1+ (0.1,0) [0|409.5] ""V""  Receiver
 SG_ VCFRONT_railA_12v m15 : 8|12@1+ (0.00544368,0) [0|22.2919] ""V""  Receiver
 SG_ VCFRONT_railA_5v m15 : 32|12@1+ (0.00544368,0) [0|22.2919] ""V""  Receiver
 SG_ VCFRONT_railB_12v m15 : 20|12@1+ (0.00544368,0) [0|22.2919] ""V""  Receiver
 SG_ VCFRONT_railB_5v m15 : 44|12@1+ (0.00544368,0) [0|22.2919] ""V""  Receiver
 SG_ VCFRONT_rightControllerCurrent m0 : 47|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_rightControllerFault m0 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_rightControllerState m0 : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_rightControllerTemp m0 : 15|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_rightControllerVoltage m0 : 31|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_sleepBypassCurrent m10 : 26|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_sleepBypassFault m10 : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_sleepBypassState m10 : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_sleepBypassVoltage m10 : 10|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_uiAudioCurrent m11 : 8|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_uiAudioFault m11 : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_uiAudioState m11 : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_uiCurrent m11 : 32|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_uiFault m11 : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_uiState m11 : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_vbatFusedHighCurrent m13 : 46|16@1+ (0.1,0) [0|6553.5] ""A""  Receiver
 SG_ VCFRONT_vbatFusedHighFault m13 : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_vbatFusedHighState m13 : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_vbatFusedHighTemp m13 : 14|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_vbatFusedHighVoltage m13 : 30|16@1+ (0.1,0) [0|6553.5] ""V""  Receiver
 SG_ VCFRONT_vbatFusedSelfTestResult m13 : 10|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCFRONT_vcleftSelfTestResult m1 : 11|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCFRONT_vcrightSelfTestResult m0 : 11|4@1+ (1,0) [0|11] """"  Receiver

BO_ 578 ID242VCLEFT_LVPowerState: 2 VehicleBus
 SG_ VCLEFT_cpLVState : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_diLVState : 6|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_lumbarLVState : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rcmLVState : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_sccmLVState : 4|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_swcLVState : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_tpmsLVState : 2|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_vehiclePowerStateDBG : 14|2@1+ (1,0) [0|3] """"  Receiver

BO_ 579 ID243VCRIGHT_hvacStatus: 8 VehicleBus
 SG_ VCRIGHT_hvacStatusIndex M : 0|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCRIGHT_hvacACRunning m0 : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacAirDistributionMode m0 : 41|3@1+ (1,0) [0|7] """"  Receiver
 SG_ VCRIGHT_hvacBlowerSegment m0 : 44|4@1+ (1,0) [0|11] """"  Receiver
 SG_ VCRIGHT_hvacCabinTempEst m0 : 30|11@1+ (0.1,-40) [-40|164] ""C""  Receiver
 SG_ VCRIGHT_hvacDuctTargetLeft m1 : 48|8@1+ (0.5,-40) [-40|80] ""C""  Receiver
 SG_ VCRIGHT_hvacDuctTargetRight m1 : 56|8@1+ (0.5,-40) [-40|80] ""C""  Receiver
 SG_ VCRIGHT_hvacEvapInletTempEstimat m2 : 19|10@1+ (0.13,-40) [-40|90] ""C""  Receiver
 SG_ VCRIGHT_hvacMassflowRefrigSystem m1 : 2|8@1+ (1,0) [0|250] ""g/s""  Receiver
 SG_ VCRIGHT_hvacModelInitStatus m0 : 60|3@1+ (1,0) [0|5] """"  Receiver
 SG_ VCRIGHT_hvacOverheatProtActive m0 : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacPowerState m0 : 51|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCRIGHT_hvacQdotLeft m0 : 2|14@1+ (1,-8191) [-8191|8191] ""W""  Receiver
 SG_ VCRIGHT_hvacQdotRight m0 : 16|14@1+ (1,-8191) [-8191|8191] ""W""  Receiver
 SG_ VCRIGHT_hvacRecirc m0 : 48|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCRIGHT_hvacRecircDoorPercent m1 : 10|6@1+ (1.6,0) [0|100] ""%""  Receiver
 SG_ VCRIGHT_hvacSecondRowState m0 : 56|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCRIGHT_hvacSystemNominal m0 : 59|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacVentStatus m0 : 54|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCRIGHT_tempDuctLeft m1 : 16|8@1+ (0.5,-22) [-22|105] ""C""  Receiver
 SG_ VCRIGHT_tempDuctLeftLower m1 : 24|8@1+ (0.5,-22) [-22|105] ""C""  Receiver
 SG_ VCRIGHT_tempDuctLeftUpper m2 : 2|8@1+ (0.5,-22) [-22|105] ""C""  Receiver
 SG_ VCRIGHT_tempDuctRight m1 : 32|8@1+ (0.5,-22) [-22|105] ""C""  Receiver
 SG_ VCRIGHT_tempDuctRightLower m1 : 40|8@1+ (0.5,-22) [-22|105] ""C""  Receiver
 SG_ VCRIGHT_tempDuctRightUpper m2 : 10|8@1+ (0.5,-22) [-22|105] ""C""  Receiver

BO_ 524 ID20CVCRIGHT_hvacRequest: 8 VehicleBus
 SG_ VCRIGHT_conditioningRequest : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_evapPerformanceLow : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacBlowerSpeedRPMReq : 32|10@1+ (5,0) [0|5115] ""RPM""  Receiver
 SG_ VCRIGHT_hvacEvapEnabled : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacHeatingEnabledLeft : 52|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacHeatingEnabledRight : 53|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacPerfTestRunning : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_hvacPerfTestState : 54|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCRIGHT_hvacUnavailable : 56|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCRIGHT_tempAmbientRaw : 44|8@1+ (0.5,-40) [-40|80] ""C""  Receiver
 SG_ VCRIGHT_tempEvaporator : 13|11@1+ (0.1,-40) [-40|105] ""C""  Receiver
 SG_ VCRIGHT_tempEvaporatorTarget : 24|8@1+ (0.2,0) [0|50] ""C""  Receiver
 SG_ VCRIGHT_wattsDemandEvap : 0|11@1+ (5,0) [0|10000] ""W""  Receiver

BO_ 737 ID2E1VCFRONT_status: 8 VehicleBus
 SG_ VCFRONT_statusIndex M : 0|3@1+ (1,0) [0|6] """"  Receiver
 SG_ VCFRONT_12VARailStable m5 : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_12VBRailStable m5 : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_12VOverchargeCounter m4 : 16|4@1+ (1,0) [0|15] """"  Receiver
 SG_ VCFRONT_5VARailStable m5 : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_5VBRailStable m5 : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_AS8510Voltage m5 : 28|12@1+ (0.00544368,0) [0|22.2864] ""V""  Receiver
 SG_ VCFRONT_ChargePumpVoltageStable m5 : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_HSDInitCompleteU13 m5 : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_HSDInitCompleteU16 m5 : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_IBSFault m4 : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_PCSMia m4 : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_PEResetLineState m5 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_airCompressorStatus m0 : 25|3@1+ (1,0) [0|7] """"  Receiver
 SG_ VCFRONT_anyClosureOpen m0 : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_anyDoorOpen m0 : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_batterySMState m1 : 16|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCFRONT_chargeNeeded m4 : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_chillerDemandActive m3 : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_compPerfRecoveryLimited m3 : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_coolantFillRoutineStatus m3 : 50|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_crashDetectedType m0 : 20|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_crashState m0 : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_crashUnlockOverrideSet m0 : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_epasWakeLine m1 : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_freezeEvapITerm m3 : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_frunkAccessPost m0 : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_frunkInteriorRelSwitch m0 : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_frunkLatchStatus m0 : 3|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCFRONT_frunkLatchType m0 : 58|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_hasLowRefrigerant m3 : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_headlampLeftFanStatus m0 : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_headlampRightFanStatus m0 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_headlightLeftVPosition m0 : 28|10@1+ (1,0) [0|1023] ""ticks""  Receiver
 SG_ VCFRONT_headlightRightVPosition m0 : 38|10@1+ (1,0) [0|1023] ""ticks""  Receiver
 SG_ VCFRONT_homelinkCommStatus m2 : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_homelinkV2Response0 m2 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_homelinkV2Response1 m2 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_homelinkV2Response2 m2 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_homelinkV2Response3 m2 : 32|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_homelinkV2Response4 m2 : 40|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_hornOn m0 : 51|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hvacModeNotAttainable m3 : 28|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hvacPerfTestCommand m3 : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_iBoosterStateDBG m1 : 5|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_iBoosterWakeLine m1 : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_isActiveHeatingBattery m0 : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_isColdStartRunning m3 : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_isEvapOperationAllowed m3 : 25|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_isHeatPumpOilPurgeActive m3 : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_maxEvapHeatRejection m3 : 8|8@1+ (65,0) [0|16575] ""W""  Receiver
 SG_ VCFRONT_minEvapHeatRejection m3 : 16|8@1+ (10,0) [0|2500] ""W""  Receiver
 SG_ VCFRONT_passengerBuckleStatus m0 : 57|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_pressureRefrigDischarge m3 : 40|8@1+ (0.125,0) [0|31.75] ""bar""  Receiver
 SG_ VCFRONT_pressureRefrigSuction m3 : 32|7@1+ (0.125,0) [0|11.5] ""bar""  Receiver
 SG_ VCFRONT_radarHeaterState m0 : 52|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_railAState m5 : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_railBState m5 : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_refrigFillRoutineStatus m3 : 52|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_reverseBatteryFault m4 : 30|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_shortedCellFaultCounter m4 : 44|3@1+ (1,0) [0|7] """"  Receiver
 SG_ VCFRONT_silentWakeIBSCurrent m4 : 32|12@1- (0.1,-204.7) [-409.5|0] ""mA""  Receiver
 SG_ VCFRONT_sleepCurrent m1 : 32|12@1- (0.1,-204.7) [-409.5|0] ""mA""  Receiver
 SG_ VCFRONT_tempCompTargetVoltage m4 : 3|10@1+ (0.01,9) [9|16] ""V""  Receiver
 SG_ VCFRONT_timeSpentSleeping m1 : 24|8@1+ (1,0) [0|255] ""s""  Receiver
 SG_ VCFRONT_vbatMonitorVoltage m5 : 16|12@1+ (0.00544368,0) [0|22.2864] ""V""  Receiver
 SG_ VCFRONT_vbatProt m5 : 40|12@1+ (0.00544368,0) [0|22.2919] ""V""  Receiver
 SG_ VCFRONT_vehicleStatusDBG m1 : 8|5@1+ (1,0) [0|17] """"  Receiver
 SG_ VCFRONT_voltageDropCounter m4 : 20|4@1+ (1,0) [0|15] """"  Receiver
 SG_ VCFRONT_voltageFloorReachedCount m4 : 24|4@1+ (1,0) [0|15] """"  Receiver
 SG_ VCFRONT_voltageProfile m4 : 28|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_wiperPosition m0 : 12|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_wiperSpeed m0 : 8|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCFRONT_wiperState m0 : 16|4@1+ (1,0) [0|12] """"  Receiver

BO_ 897 ID381VCFRONT_logging1Hz: 8 VehicleBus
 SG_ VCFRONT_logging1HzIndex M : 0|5@1+ (1,0) [0|19] """"  Receiver
 SG_ VCFRONT_CCQdotActual m9 : 21|7@1+ (80,0) [0|10100] ""W""  Receiver
 SG_ VCFRONT_CCQdotFdFrwrdTarget m9 : 7|7@1+ (80,0) [0|10100] ""W""  Receiver
 SG_ VCFRONT_CCQdotFdbk m9 : 14|7@1- (80,0) [-5000|5000] ""W""  Receiver
 SG_ VCFRONT_CMPDischargeSuperheat m8 : 17|5@1- (1,6) [-10|21] ""C""  Receiver
 SG_ VCFRONT_DIQdotA m9 : 42|7@1+ (80,0) [0|10000] ""W""  Receiver
 SG_ VCFRONT_HCML_bladeTemp m3 : 32|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCML_diffuseTemp m3 : 40|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCML_highBeamTemp m3 : 16|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCML_lowBeamSpotTemp m3 : 8|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCML_turnTemp m3 : 24|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCMR_bladeTemp m4 : 32|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCMR_diffuseTemp m4 : 40|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCMR_highBeamTemp m4 : 16|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCMR_lowBeamSpotTemp m4 : 8|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_HCMR_turnTemp m4 : 24|8@1- (1,67) [-60|194] ""C""  Receiver
 SG_ VCFRONT_PDischargeControllerOutp m14 : 24|8@1+ (0.5,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_PSuctionControllerOutput m14 : 40|8@1+ (0.5,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_ambientColderThanBatt m16 : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_ambientSourcingAvailable m16 : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_ambientSourcingDisabled m14 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_battDissipation m12 : 8|8@1+ (140,0) [0|35000] ""W""  Receiver
 SG_ VCFRONT_battLoopWorthCooling m16 : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_battOverStagUpperLimit m16 : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_battUnderStagUpperLimit m16 : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_cabinHeatEnergyDuringDri m8 : 42|7@1+ (150,0) [0|19000] ""Wh""  Receiver
 SG_ VCFRONT_calibratedPositionHCML m6 : 5|10@1+ (1,0) [0|1023] ""Steps""  Receiver
 SG_ VCFRONT_calibratedPositionHCMR m6 : 16|10@1+ (1,0) [0|1023] ""Steps""  Receiver
 SG_ VCFRONT_ccLeftExvCalibFailed m11 : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_ccLeftExvCalibOffset m11 : 40|8@1- (4,0) [-510|510] ""ticks""  Receiver
 SG_ VCFRONT_ccLeftExvRange m7 : 41|9@1+ (3,0) [0|1500] ""ticks""  Receiver
 SG_ VCFRONT_ccRightExvCalibFailed m11 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_ccRightExvCalibOffset m11 : 48|8@1- (4,0) [-510|510] ""ticks""  Receiver
 SG_ VCFRONT_ccRightExvRange m7 : 50|9@1+ (3,0) [0|1500] ""ticks""  Receiver
 SG_ VCFRONT_chillerExvCalibFailed m11 : 56|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_chillerExvCalibOffset m11 : 8|8@1- (4,0) [-510|510] ""ticks""  Receiver
 SG_ VCFRONT_chillerExvRange m7 : 5|9@1+ (3,0) [0|1500] ""ticks""  Receiver
 SG_ VCFRONT_chillerInletTempEstimate m13 : 27|8@1- (1,0) [-40|85] ""C""  Receiver
 SG_ VCFRONT_chillerLiftDisabledLowPs m14 : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_chillerPassiveCoolPower m12 : 24|8@1- (60,7560) [0|15000] ""W""  Receiver
 SG_ VCFRONT_compEnergyDuringDrive m8 : 35|7@1+ (150,0) [0|19000] ""Wh""  Receiver
 SG_ VCFRONT_condenserPressureLimit m1 : 8|6@1+ (0.16,10) [10|20] ""bar""  Receiver
 SG_ VCFRONT_coolantLevelVoltage m0 : 55|9@1+ (0.01,0) [0|5] ""V""  Receiver
 SG_ VCFRONT_coolantValveAngleDrift m2 : 18|10@1+ (0.25,-127) [-127|127] ""degrees""  Receiver
 SG_ VCFRONT_coolantValveCountRange m2 : 8|10@1+ (1,375) [375|1375] ""ticks""  Receiver
 SG_ VCFRONT_coolantValveDailyAngleTr m19 : 34|10@1+ (20,0) [0|20000] ""degrees""  Receiver
 SG_ VCFRONT_coolantValveOdometer m19 : 24|10@1+ (500,0) [0|500000] ""degrees""  Receiver
 SG_ VCFRONT_coolantValveRadBypass m2 : 56|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_coolantValveRecalCount m2 : 28|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ VCFRONT_coolantValveRecalReason m2 : 5|3@1+ (1,0) [0|7] """"  Receiver
 SG_ VCFRONT_coolantValveWindupEstL m2 : 44|6@1+ (2,0) [0|126] ""ticks""  Receiver
 SG_ VCFRONT_coolantValveWindupEstR m2 : 50|6@1+ (2,0) [0|126] ""ticks""  Receiver
 SG_ VCFRONT_currentPositionHCML m6 : 26|10@1+ (1,0) [0|1023] ""Steps""  Receiver
 SG_ VCFRONT_currentPositionHCMR m6 : 36|10@1+ (1,0) [0|1023] ""Steps""  Receiver
 SG_ VCFRONT_cycleModelConverged m10 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_diDissipation m12 : 16|8@1+ (80,0) [0|20000] ""W""  Receiver
 SG_ VCFRONT_dischargePressureLimit m19 : 48|7@1+ (0.25,0) [0|26] ""bar""  Receiver
 SG_ VCFRONT_dischargePressureTarget m14 : 8|7@1+ (0.25,0) [0|26] ""bar""  Receiver
 SG_ VCFRONT_drlMode m18 : 5|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_estCompPower m16 : 35|8@1+ (50,0) [0|12750] ""W""  Receiver
 SG_ VCFRONT_estCompRefrigMassflow m16 : 17|6@1+ (4,0) [0|251] ""g/s""  Receiver
 SG_ VCFRONT_estCompressorRpm m10 : 48|6@1+ (400,0) [0|25200] ""rpm""  Receiver
 SG_ VCFRONT_estPressureDisch m10 : 19|7@1+ (0.25,0) [0|31.75] ""bar""  Receiver
 SG_ VCFRONT_estPressureLiq m10 : 5|7@1+ (0.25,0) [0|31.75] ""bar""  Receiver
 SG_ VCFRONT_estPressureSuct m10 : 12|7@1+ (0.125,0) [0|11.5] ""bar""  Receiver
 SG_ VCFRONT_estQLift m10 : 54|7@1+ (100,0) [0|12700] ""W""  Receiver
 SG_ VCFRONT_estTempDisch m10 : 40|8@1- (0.8,68.8) [-34|170] ""C""  Receiver
 SG_ VCFRONT_estTempLiq m10 : 26|8@1- (0.8,68.8) [-34|170] ""C""  Receiver
 SG_ VCFRONT_estTempSuct m10 : 34|6@1- (1,2) [-30|33] ""C""  Receiver
 SG_ VCFRONT_evapDisabledLowPsCutout m14 : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_evapExvCalibFailed m11 : 57|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_evapExvCalibOffset m11 : 16|8@1- (4,0) [-510|510] ""ticks""  Receiver
 SG_ VCFRONT_evapExvRange m7 : 14|9@1+ (3,0) [0|1500] ""ticks""  Receiver
 SG_ VCFRONT_evapFdFrwrdTarget m9 : 28|7@1+ (80,0) [0|10100] ""W""  Receiver
 SG_ VCFRONT_evapFdFrwrdTargetMinimum m9 : 49|7@1+ (80,0) [0|10000] ""W""  Receiver
 SG_ VCFRONT_evapFdbk m9 : 35|7@1- (80,0) [-5000|5000] ""W""  Receiver
 SG_ VCFRONT_exteriorQuietModeAllowed m9 : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_exteriorQuietModeEnabled m9 : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_fanControlFeedfwdActive m15 : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_fanControlRadCanCool m15 : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_fanControlRadiatorInletT m15 : 57|6@1- (1.8,36) [-20|90] ""C""  Receiver
 SG_ VCFRONT_fanControlRadiatorUa m15 : 50|7@1+ (8,0) [0|1000] ""W/C""  Receiver
 SG_ VCFRONT_fanDemandCondenser m1 : 14|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_fanDemandRadiator m1 : 21|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_feedBackDuctTempControll m15 : 32|8@1+ (1,-100) [-100|100] ""-""  Receiver
 SG_ VCFRONT_feedBackEvapTempControll m15 : 24|7@1+ (0.01,-0.5) [-0.5|0.5] ""-""  Receiver
 SG_ VCFRONT_feedFwdFanDemand m13 : 35|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_feedFwdMDotCabinCondense m15 : 16|8@1+ (0.005,0) [0|1] ""-""  Receiver
 SG_ VCFRONT_feedFwdMDotEvaporator m15 : 8|8@1+ (0.005,0) [0|1] ""-""  Receiver
 SG_ VCFRONT_feedFwdPumpDemand m13 : 42|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_feedfwdLoadCoolingDomina m9 : 59|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_homelinkRegionCode m5 : 8|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_hpAmbientSource m17 : 6|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpAtSteadyState m16 : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpBattOverTempHvacDisabl m17 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpBattStagTarget m8 : 28|7@1+ (1,-40) [-40|80] ""C""  Receiver
 SG_ VCFRONT_hpBatteryCool m17 : 19|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpBatteryCoolCabinConden m17 : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpBatteryCoolCabinReheat m17 : 21|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpBatteryCoolEvaporator m17 : 22|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpBatteryHeatAmbientSour m17 : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpBatteryHeatCOP1 m17 : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCOP m8 : 22|6@1+ (0.1,0) [0|6] ""-""  Receiver
 SG_ VCFRONT_hpCabinCoolEvaporator m17 : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinCoolEvaporatorReh m17 : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatAmbientSource m17 : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatBatteryCoolRe m17 : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatBatteryHeatRe m17 : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatBlend m17 : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatCOP1 m17 : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatReheatAmbient m17 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatReheatScaveng m17 : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCabinHeatScavengeOnly m17 : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpCompPowerIndex m16 : 43|7@1+ (1,0) [0|127] ""-""  Receiver
 SG_ VCFRONT_hpCompPowerIndexFiltered m12 : 56|7@1+ (1,0) [0|127] ""-""  Receiver
 SG_ VCFRONT_hpDiagLouverCalib m17 : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpForceScavenge m16 : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpGeneral m17 : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpMode m16 : 10|5@1+ (1,0) [0|19] """"  Receiver
 SG_ VCFRONT_hpPotentialLowRefrig m9 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpRefrigerantPurgeState m9 : 62|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_hpSubcoolTarget m8 : 12|5@1+ (1,0) [0|31] ""C""  Receiver
 SG_ VCFRONT_lccActiveCoolTarget m19 : 16|7@1+ (1,0) [0|100] ""C""  Receiver
 SG_ VCFRONT_lccExvCalibFailed m11 : 59|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_lccExvCalibOffset m11 : 32|8@1- (4,0) [-510|510] ""ticks""  Receiver
 SG_ VCFRONT_lccExvRange m7 : 32|9@1+ (3,0) [0|1500] ""ticks""  Receiver
 SG_ VCFRONT_lccInletTempEstimate m13 : 19|8@1- (1,0) [-40|85] ""C""  Receiver
 SG_ VCFRONT_lccPassiveHeatPower m12 : 32|8@1+ (60,0) [0|15000] ""W""  Receiver
 SG_ VCFRONT_lowSideLiftEnergyDrive m8 : 49|7@1+ (150,0) [0|19000] ""Wh""  Receiver
 SG_ VCFRONT_lowSideWattsLift m8 : 56|7@1+ (60,0) [0|7600] ""W""  Receiver
 SG_ VCFRONT_maxAllowedEvapPowerInSer m12 : 40|8@1+ (40,0) [0|10000] ""W""  Receiver
 SG_ VCFRONT_maxChillerCoolingPower m15 : 40|8@1+ (100,0) [0|20000] ""W""  Receiver
 SG_ VCFRONT_maxCompressorRPMAllowed m14 : 56|7@1+ (90,0) [0|11430] ""rpm""  Receiver
 SG_ VCFRONT_minAllowedChillerPowerIn m12 : 48|8@1+ (40,0) [0|10000] ""W""  Receiver
 SG_ VCFRONT_minAllowedSuctionPressur m14 : 48|7@1+ (0.02,0) [0|2.5] ""bar""  Receiver
 SG_ VCFRONT_minFlowPDCont m14 : 16|8@1+ (0.5,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_modeDesired m0 : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_modeTransitionID m0 : 8|6@1+ (1,0) [0|40] """"  Receiver
 SG_ VCFRONT_modelLoadCoolingDominant m9 : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_passiveCoolingState m9 : 56|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_passiveDemandRadBypass m19 : 8|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_passiveSeriesRegOn m19 : 5|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_pressureRefrigDischEst m16 : 24|5@1+ (1,0) [0|31] ""bar""  Receiver
 SG_ VCFRONT_pressureRefrigSuctionEst m16 : 51|7@1+ (0.12,0) [0|14.36] ""bar""  Receiver
 SG_ VCFRONT_ptLoopWorthCooling m16 : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_pumpBatteryFETTemp m1 : 36|8@1+ (1,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_pumpPowertrainFETTemp m1 : 44|8@1+ (1,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_radActiveRejectEstimate m13 : 49|15@1+ (1,0) [0|30000] ""W""  Receiver
 SG_ VCFRONT_radPassiveRejectEstimate m13 : 5|14@1+ (1,0) [0|15000] ""W""  Receiver
 SG_ VCFRONT_radiatorFanFETTemp m1 : 52|8@1+ (1,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_radiatorFanRunReason m1 : 60|4@1+ (1,0) [0|8] """"  Receiver
 SG_ VCFRONT_recircExvCalibFailed m11 : 58|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_recircExvCalibOffset m11 : 24|8@1- (4,0) [-510|510] ""ticks""  Receiver
 SG_ VCFRONT_recircExvRange m7 : 23|9@1+ (3,0) [0|1500] ""ticks""  Receiver
 SG_ VCFRONT_subcoolActual m8 : 5|7@1- (0.4,15.2) [-10|40] ""C""  Receiver
 SG_ VCFRONT_suctionPressureTarget m14 : 32|6@1+ (0.15,0) [0|8] ""bar""  Receiver
 SG_ VCFRONT_suctionSuperheatEstPsSNA m16 : 58|4@1+ (2,0) [0|30] ""C""  Receiver
 SG_ VCFRONT_suctionSuperheatEstTsSNA m17 : 24|4@1+ (2,0) [0|30] ""C""  Receiver
 SG_ VCFRONT_targetBatActiveCool m0 : 32|7@1+ (1,0) [0|100] ""C""  Receiver
 SG_ VCFRONT_targetBatActiveHeat m0 : 48|7@1+ (1,-40) [-40|60] ""C""  Receiver
 SG_ VCFRONT_targetBatPassive m0 : 40|7@1+ (1,-20) [-20|80] ""C""  Receiver
 SG_ VCFRONT_targetPTActiveCool m0 : 16|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ VCFRONT_targetPTPassive m0 : 24|8@1+ (1,-40) [-40|120] ""C""  Receiver
 SG_ VCFRONT_tempRefrigDischargeEst m16 : 29|6@1+ (2,0) [0|125] ""C""  Receiver
 SG_ VCFRONT_tempRefrigSuction m1 : 28|8@1+ (0.5,-40) [-40|85] ""C""  Receiver
 SG_ VCFRONT_tempRefrigSuctionEst m17 : 32|6@1+ (1,0) [0|63] ""C""  Receiver
 SG_ VCFRONT_totalLoadCoolingDominant m9 : 58|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_wiperCycles m18 : 48|16@1+ (100,0) [0|6553500] ""Cycles""  Receiver

BO_ 792 ID318SystemTimeUTC: 8 VehicleBus
 SG_ UTCyear318 : 0|8@1+ (1,0) [0|0] ""yr""  Receiver
 SG_ UTCmonth318 : 8|8@1+ (1,0) [0|0] ""mo""  Receiver
 SG_ UTCseconds318 : 16|8@1+ (1,0) [0|0] ""sec""  Receiver
 SG_ UTChour318 : 24|8@1+ (1,0) [0|0] ""hr""  Receiver
 SG_ UTCday318 : 32|8@1+ (1,0) [0|0] ""dy""  Receiver
 SG_ UTCminutes318 : 40|8@1+ (1,0) [0|0] ""min""  Receiver

BO_ 1320 ID528UnixTime: 4 VehicleBus
 SG_ UnixTimeSeconds528 : 7|32@0+ (1,0) [0|4294970000] ""sec""  Receiver

BO_ 553 ID229GearLever: 3 VehicleBus
 SG_ GearLeverPosition229 : 12|3@1+ (1,0) [0|7] """"  Receiver
 SG_ GearLeverButton229 : 16|2@1+ (1,0) [0|3] """"  Receiver

BO_ 585 ID249SCCMLeftStalk: 4 VehicleBus
 SG_ SCCM_highBeamStalkStatus : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ SCCM_leftStalkCounter : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ SCCM_leftStalkCrc : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ SCCM_leftStalkReserved1 : 24|5@1+ (1,0) [0|31] """"  Receiver
 SG_ SCCM_turnIndicatorStalkStatus : 16|4@1+ (1,0) [0|9] """"  Receiver
 SG_ SCCM_washWipeButtonStatus : 14|2@1+ (1,0) [0|3] """"  Receiver

BO_ 390 ID186DIF_torque: 8 VehicleBus
 SG_ DIF_axleSpeed : 40|16@1- (0.1,0) [-2750|2750] ""RPM""  Receiver
 SG_ DIF_slavePedalPos : 56|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIF_torqueActual : 27|13@1- (2,0) [-7500|7500] ""Nm""  Receiver
 SG_ DIF_torqueChecksum : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIF_torqueCommand : 12|13@1- (2,0) [-7500|7500] ""Nm""  Receiver
 SG_ DIF_torqueCounter : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DIF_axleSpeedQF : 25|1@1+ (1,0) [0|1] """"  Receiver

BO_ 918 ID396FrontOilPump: 8 VehicleBus
 SG_ FrontOilPumpPressureExpected396 : 48|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ FrontOilPumpPhaseCurrent396 : 56|8@1+ (0.1,0) [0|25.4] ""A""  Receiver
 SG_ FrontOilFlowActual396 : 16|8@1+ (0.06,0) [0|15] ""LPM""  Receiver
 SG_ FrontOilPumpDutyCycle396 : 8|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ FrontOilPumpOilTempEst396 : 32|8@1+ (1,-40) [0|214] ""C""  Receiver
 SG_ FrontOilPumpFluidTemp396 : 24|8@1+ (1,-40) [0|214] ""C""  Receiver
 SG_ FrontOilPumpState396 : 0|3@1+ (1,0) [0|7] """"  Receiver
 SG_ FrontOilPumpPressureEstimate396 : 40|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ FrontOilPumpOilTempEstConfident3 : 3|1@1+ (1,0) [0|0] """"  Receiver
 SG_ FrontOilPumpLeadAngle396 : 4|4@1+ (1.875,0) [0|28.125] ""deg""  Receiver

BO_ 917 ID395DIR_oilPump: 8 VehicleBus
 SG_ DIR_oilPumpFlowActual : 16|8@1+ (0.06,0) [0|15] ""LPM""  Receiver
 SG_ DIR_oilPumpFlowTarget : 8|8@1+ (0.06,0) [0|15] ""LPM""  Receiver
 SG_ DIR_oilPumpFluidT : 24|8@1+ (1,-40) [-40|214] ""C""  Receiver
 SG_ DIR_oilPumpFluidTQF : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_oilPumpLeadAngle : 4|4@1+ (1.875,0) [0|28.125] ""deg""  Receiver
 SG_ DIR_oilPumpPhaseCurrent : 32|8@1+ (0.1,0) [0|25.4] ""A""  Receiver
 SG_ DIR_oilPumpPressureEstimate : 40|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ DIR_oilPumpPressureExpected : 48|8@1+ (2,0) [0|500] ""kPa""  Receiver
 SG_ DIR_oilPumpPressureResidual : 56|8@1+ (4,-500) [-500|500] ""kPa""  Receiver
 SG_ DIR_oilPumpState : 0|3@1+ (1,0) [0|7] """"  Receiver

BO_ 472 ID1D8RearTorque: 8 VehicleBus
 SG_ Checksum1D8 : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ Counter1D8 : 53|3@1+ (1,0) [0|7] """"  Receiver
 SG_ RearTorqueRequest1D8 : 8|13@1- (0.222,0) [-909.312|909.09] ""NM""  Receiver
 SG_ RearTorque1D8 : 21|13@1- (0.222,0) [-909.312|909.09] ""NM""  Receiver
 SG_ TorqueFlags1D8 : 0|8@1+ (1,0) [0|255] """"  Receiver

BO_ 341 ID155WheelAngles: 8 ChassisBus
 SG_ WheelAngleTicsRR155 : 24|8@1+ (1.40625,0) [0|358.594] ""deg""  Receiver
 SG_ WheelAngleTicsRL155 : 16|8@1+ (1.40625,0) [0|358.594] ""deg""  Receiver
 SG_ WheelAngleTicsFR155 : 8|8@1+ (1.40625,0) [0|358.594] ""deg""  Receiver
 SG_ WheelAngleTicsFL155 : 0|8@1+ (1.40625,0) [0|358.594] ""deg""  Receiver
 SG_ ESP_WheelRotationReR : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_WheelRotationReL : 34|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_WheelRotationFrR : 36|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_WheelRotationFrL : 38|2@1+ (1,0) [0|3] """"  Receiver
 SG_ ESP_wheelSpeedsQF : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_vehicleStandstillSts : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_vehicleSpeed : 42|10@1+ (0.5,0) [0|511] ""kph""  Receiver
 SG_ ESP_wheelRotationCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ ESP_wheelRotationChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver

BO_ 373 ID175WheelSpeed: 8 ChassisBus
 SG_ WheelSpeedRR175 : 39|13@1+ (0.04,0) [0|327.64] ""KPH""  Receiver
 SG_ WheelSpeedRL175 : 26|13@1+ (0.04,0) [0|327.64] ""KPH""  Receiver
 SG_ WheelSpeedFR175 : 13|13@1+ (0.04,0) [0|327.64] ""KPH""  Receiver
 SG_ WheelSpeedFL175 : 0|13@1+ (0.04,0) [0|327.64] ""KPH""  Receiver
 SG_ ESP_wheelSpeedsCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ ESP_wheelSpeedsChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver

BO_ 389 ID185ESP_brakeTorque: 8 ChassisBus
 SG_ ESP_brakeTorqueFrL : 0|12@1+ (2,0) [0|8190] ""Nm""  Receiver
 SG_ ESP_brakeTorqueFrR : 12|12@1+ (2,0) [0|8190] ""Nm""  Receiver
 SG_ ESP_brakeTorqueReL : 24|12@1+ (2,0) [0|8190] ""Nm""  Receiver
 SG_ ESP_brakeTorqueReR : 36|12@1+ (2,0) [0|8190] ""Nm""  Receiver
 SG_ ESP_brakeTorqueQF : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ ESP_brakeTorqueCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ ESP_brakeTorqueChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver

BO_ 468 ID1D4FrontTorqueOld: 8 VehicleBus
 SG_ RAWTorqueFront1D4 : 40|12@1- (0.25,0) [-512|511.75] ""NM""  Receiver

BO_ 469 ID1D5FrontTorque: 8 VehicleBus
 SG_ FrontTorqueRequest1D5 : 8|13@1- (0.222,0) [-909.312|909.09] ""NM""  Receiver
 SG_ FrontTorque1D5 : 21|13@1- (0.222,0) [-909.312|909.09] ""NM""  Receiver

BO_ 641 ID281VCFRONT_CMPRequest: 8 VehicleBus
 SG_ VCFRONT_CMPTargetDuty : 0|16@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_CMPPowerLimit : 16|16@1+ (1,0) [0|65534] ""W""  Receiver
 SG_ VCFRONT_CMPReset : 32|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_CMPEnable : 40|1@1+ (1,0) [0|1] """"  Receiver

BO_ 962 ID3C2VCLEFT_switchStatus: 8 VehicleBus
 SG_ VCLEFT_switchStatusIndex M : 0|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCLEFT_2RowSeatBothFoldFlatSwitc m1 : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_2RowSeatCenterSwitch m1 : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_2RowSeatLeftFoldFlatSwitc m1 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_2RowSeatReclineSwitch m1 : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_2RowSeatRightFoldFlatSwit m1 : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_brakePressed m0 : 60|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_brakeSwitchPressed m0 : 4|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowAutoDownLR m1 : 35|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowAutoUpLR m1 : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowDownLR m1 : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoDownLF m0 : 35|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoDownLR m0 : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoDownRF m0 : 43|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoDownRR m0 : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoUpLF m0 : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoUpLR m0 : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoUpRF m0 : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackAutoUpRR m0 : 45|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackDownLF m0 : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackDownLR m0 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackDownRF m0 : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackDownRR m0 : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackUpLF m0 : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackUpLR m0 : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackUpRF m0 : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowSwPackUpRR m0 : 44|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_btnWindowUpLR m1 : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_frontBuckleSwitch m0 : 48|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontOccupancySwitch m0 : 50|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatBackrestBack m0 : 20|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatBackrestForward m0 : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatLiftDown m0 : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatLiftUp m0 : 18|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatLumbarDown m0 : 24|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatLumbarIn m0 : 28|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatLumbarOut m0 : 30|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatLumbarUp m0 : 26|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatTiltDown m0 : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatTiltUp m0 : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatTrackBack m0 : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_frontSeatTrackForward m0 : 10|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_hazardButtonPressed m0 : 3|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_hornSwitchPressed m0 : 2|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_rearCenterBuckleSwitch m0 : 62|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rearCenterOccupancySwitch m0 : 54|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rearHVACButtonPressed m0 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_rearLeftBuckleSwitch m0 : 52|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rearLeftOccupancySwitch m0 : 56|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rearRightOccupancySwitch m0 : 58|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_rightMirrorTilt m0 : 5|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCLEFT_swcLeftDoublePress m1 : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_swcLeftPressed m1 : 5|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_swcLeftScrollTicks m1 : 16|6@1- (1,0) [-32|31] """"  Receiver
 SG_ VCLEFT_swcLeftTiltLeft m1 : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_swcLeftTiltRight m1 : 3|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_swcRightDoublePress m1 : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCLEFT_swcRightPressed m1 : 12|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_swcRightScrollTicks m1 : 24|6@1- (1,0) [-32|31] """"  Receiver
 SG_ VCLEFT_swcRightTiltLeft m1 : 8|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCLEFT_swcRightTiltRight m1 : 10|2@1+ (1,0) [0|3] """"  Receiver

BO_ 822 ID336MaxPowerRating: 4 VehicleBus
 SG_ DriveRegenRating336 : 16|8@1+ (1,-100) [-100|155] ""kW""  Receiver
 SG_ DrivePowerRating336 : 0|10@1+ (1,0) [0|600] ""kW""  Receiver
 SG_ DI_performancePackage : 24|3@1+ (1,0) [0|4] """"  Receiver

BO_ 659 ID293UI_chassisControl: 8 VehicleBus
 SG_ UI_accOvertakeEnable : 16|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_aebEnable : 18|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_aesEnable : 20|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_ahlbEnable : 22|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_autoLaneChangeEnable : 24|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_autoParkRequest : 28|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_bsdEnable : 32|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_chassisControlChecksum : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ UI_chassisControlCounter : 52|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_dasDebugEnable : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_distanceUnits : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_fcwEnable : 34|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_fcwSensitivity : 36|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_latControlEnable : 38|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_ldwEnable : 40|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_narrowGarages : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_parkBrakeRequest : 5|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_pedalSafetyEnable : 42|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_rebootAutopilot : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_redLightStopSignEnable : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ UI_selfParkTune : 48|4@1+ (1,0) [0|15] """"  Receiver
 SG_ UI_steeringTuneRequest : 0|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_tractionControlMode : 2|3@1+ (1,0) [0|6] """"  Receiver
 SG_ UI_trailerMode : 12|1@1+ (1,0) [0|1] """"  Receiver
 SG_ UI_winchModeRequest : 8|2@1+ (1,0) [0|2] """"  Receiver
 SG_ UI_zeroSpeedConfirmed : 10|2@1+ (1,0) [0|3] """"  Receiver

BO_ 616 ID268SystemPower: 5 VehicleBus
 SG_ SystemRegenPowerMax268 : 32|8@1+ (1,-100) [-100|155] ""kW""  Receiver
 SG_ SystemHeatPowerMax268 : 0|8@1+ (0.08,0) [0|31875] ""kW""  Receiver
 SG_ SystemHeatPower268 : 8|8@1+ (0.08,0) [0|31875] ""kW""  Receiver
 SG_ SystemDrivePowerMax268 : 16|9@1+ (1,0) [0|511] ""kW""  Receiver
 SG_ DI_primaryUnitSiliconType : 26|1@1+ (1,0) [0|1] """"  Receiver

BO_ 79 ID04FGPSLatLong: 8 ChassisBus
 SG_ GPSAccuracy04F : 57|7@1+ (0.2,0) [0|25.2] ""m""  Receiver
 SG_ GPSLongitude04F : 28|28@1- (1E-006,0) [-134.218|134.218] ""Deg""  Receiver
 SG_ GPSLatitude04F : 0|28@1- (1E-006,0) [-134.218|134.218] ""Deg""  Receiver

BO_ 978 ID3D2TotalChargeDischarge: 8 VehicleBus
 SG_ TotalDischargeKWh3D2 : 0|32@1+ (0.001,0) [0|4294970] ""kWh""  Receiver
 SG_ TotalChargeKWh3D2 : 32|32@1+ (0.001,0) [0|4294970] ""kWh""  Receiver

BO_ 1010 ID3F2BMSCounters: 8 VehicleBus
 SG_ BMS_kwhCounter_Id M : 0|4@1+ (1,0) [0|0] """"  Receiver
 SG_ BMS_kwhDcChargeTotalModule3 m9 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhChargeTotalModule2 m6 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDcChargeTotalModule2 m7 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhChargeTotalModule4 m10 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDcChargeTotalModule4 m11 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDcChargeTotalModule1 m5 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhChargeTotalModule1 m4 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhChargeTotalModule3 m8 : 36|28@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhAcChargeTotalModule3 m9 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDischargeTotalModule2 m6 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhAcChargeTotalModule2 m7 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDischargeTotalModule4 m10 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhAcChargeTotalModule4 m11 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhAcChargeTotalModule1 m5 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDriveDischargeTotal m3 : 8|32@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_dcChargerKwhTotal m1 : 8|32@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDischargeTotalModule1 m4 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_acChargerKwhTotal m0 : 8|32@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhRegenChargeTotal m2 : 8|32@1+ (0.001,0) [0|0] ""KWh""  Receiver
 SG_ BMS_kwhDischargeTotalModule3 m8 : 8|28@1- (0.001,0) [0|0] ""KWh""  Receiver

BO_ 722 ID2D2BMSVAlimits: 8 VehicleBus
 SG_ MinVoltage2D2 : 0|16@1+ (0.01,0) [0|430] ""V""  Receiver
 SG_ MaxDischargeCurrent2D2 : 48|14@1+ (0.128,0) [0|2096.9] ""A""  Receiver
 SG_ MaxChargeCurrent2D2 : 32|14@1+ (0.1,0) [0|1638.2] ""A""  Receiver
 SG_ MaxVoltage2D2 : 16|16@1+ (0.01,0) [0|430] ""V""  Receiver

BO_ 1345 ID541FastChargeMaxLimits: 8 VehicleBus
 SG_ FCMaxPowerLimit541 : 0|13@1+ (0.062256,0) [0|509.939] ""kW""  Receiver
 SG_ FCMaxCurrentLimit541 : 16|13@1+ (0.073242,0) [0|599.925] ""A""  Receiver

BO_ 580 ID244FastChargeLimits: 8 VehicleBus
 SG_ FCMinVlimit244 : 48|13@1+ (0.073242,0) [0|599.925] ""V""  Receiver
 SG_ FCMaxVlimit244 : 32|13@1+ (0.073242,0) [0|599.925] ""V""  Receiver
 SG_ FCCurrentLimit244 : 16|13@1+ (0.073242,0) [0|599.925] ""A""  Receiver
 SG_ FCPowerLimit244 : 0|13@1+ (0.062256,0) [0|509.939] ""kW""  Receiver

BO_ 532 ID214FastChargeVA: 8 VehicleBus
 SG_ FC_adapterLocked : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_dcCurrent : 32|14@1- (0.0732422,0) [-600|599.927] ""A""  Receiver
 SG_ FC_dcVoltage : 48|13@1+ (0.0732422,0) [0|599.927] ""V""  Receiver
 SG_ FC_leakageTestNotSupported : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_minCurrentLimit : 16|13@1+ (0.1,0) [0|600] ""A""  Receiver
 SG_ FC_postID : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ FC_protocolVersion : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_statusCode : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ FC_type : 29|3@1+ (1,0) [0|7] """"  Receiver

BO_ 533 ID215FCisolation: 1 VehicleBus
 SG_ FCIsolation215 : 0|8@1+ (40,0) [0|10200] ""kOhm""  Receiver

BO_ 535 ID217FC_status3: 8 VehicleBus
 SG_ FC_status3DataSelect M : 0|7@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_billingEnergy m1 : 8|16@1+ (0.015259,0) [0|1000] ""kWh""  Receiver
 SG_ FC_brand m0 : 16|4@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_class m0 : 8|8@1+ (1,0) [0|2] """"  Receiver
 SG_ FC_coolingType m0 : 20|4@1+ (1,0) [0|2] """"  Receiver
 SG_ FC_generation m0 : 32|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_status3DummySig : 7|1@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_uiStopType m0 : 24|4@1+ (1,0) [0|2] """"  Receiver

BO_ 801 ID321VCFRONT_sensors: 8 VehicleBus
 SG_ VCFRONT_battSensorIrrational : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_brakeFluidLevel : 22|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_coolantLevel : 21|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_ptSensorIrrational : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_tempAmbient : 24|8@1+ (0.5,-40) [-40|80] ""C""  Receiver
 SG_ VCFRONT_tempAmbientFiltered : 40|8@1+ (0.5,-40) [-40|80] ""C""  Receiver
 SG_ VCFRONT_tempCoolantBatInlet : 0|10@1+ (0.125,-40) [-40|85] ""C""  Receiver
 SG_ VCFRONT_tempCoolantPTInlet : 10|11@1+ (0.125,-40) [-40|200] ""C""  Receiver
 SG_ VCFRONT_washerFluidLevel : 32|2@1+ (1,0) [0|2] """"  Receiver

BO_ 769 ID301VCFRONT_info: 8 VehicleBus
 SG_ VCFRONT_infoIndex M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_hcmlAppCRC m16 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_hcmrAppCRC m23 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_infoAppCrc m13 : 8|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_infoAppGitHash m17 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ VCFRONT_infoAssemblyId m11 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_infoBootCrc m20 : 24|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_infoBootGitHash m18 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ VCFRONT_infoBootUdsProtoVersion m20 : 8|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ VCFRONT_infoBuildConfigId m10 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ VCFRONT_infoBuildType m10 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_infoComponentId m10 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ VCFRONT_infoHardwareId m10 : 32|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ VCFRONT_infoPcbaId m11 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_infoPlatformType m19 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ VCFRONT_infoSubUsageId m11 : 40|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ VCFRONT_infoSubcomponent3 m24 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_infoSubcomponent4 m31 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_infoSubcomponent5 m32 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_infoSubcomponent6 m33 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ VCFRONT_infoUsageId m11 : 24|16@1+ (1,0) [0|65535] """"  Receiver

BO_ 513 ID201VCFRONT_loggingAndVitals10H: 8 VehicleBus
 SG_ VCFRONT_loggingAndVitals10HzInde M : 0|3@1+ (1,0) [0|7] """"  Receiver
 SG_ VCFRONT_activeLouverOpenPos m1 : 16|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_activeLouverOpenPosTarg m1 : 8|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_cclExvFlow m3 : 44|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_cclExvFlowTarget m5 : 44|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_cclExvState m6 : 20|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_ccrExvFlow m3 : 54|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_ccrExvFlowTarget m5 : 54|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_ccrExvState m6 : 24|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_chillerExvFlow m3 : 3|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_chillerExvFlowTarget m5 : 3|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_chillerExvState m6 : 3|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_compDemandChiller m2 : 39|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_compDemandEvap m2 : 26|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_compressorState m2 : 24|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_coolantValveMode m2 : 61|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_evapExvFlow m3 : 13|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_evapExvFlowTarget m5 : 13|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_evapExvState m6 : 8|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_exvFlowTarget m0 : 54|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_exvState m2 : 33|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_fanDemand m2 : 16|7@1+ (1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_hpBatteryLoadType m4 : 16|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_hpBlendType m4 : 11|2@1+ (1,0) [0|2] """"  Receiver
 SG_ VCFRONT_hpCabinLoadType m4 : 14|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_hpDominantLoad m4 : 8|3@1+ (1,0) [0|6] """"  Receiver
 SG_ VCFRONT_hpHighSideHX m4 : 3|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_hpLowSideHX m4 : 5|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_hpQuietModeEnabled m4 : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ VCFRONT_hpReqCoolantMode m4 : 18|3@1+ (1,0) [0|4] """"  Receiver
 SG_ VCFRONT_lccExvFlow m3 : 34|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_lccExvFlowTarget m5 : 34|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_lccExvState m6 : 16|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_pressureRefrigDischargeV m2 : 53|8@1+ (0.125,0) [0|31.75] ""bar""  Receiver
 SG_ VCFRONT_pressureRefrigLiquid m1 : 40|8@1+ (0.125,0) [0|31.75] ""bar""  Receiver
 SG_ VCFRONT_pressureRefrigSuctionVit m2 : 46|7@1+ (0.125,0) [0|11.5] ""bar""  Receiver
 SG_ VCFRONT_pumpBatteryRPMActual m0 : 3|10@1+ (10,0) [0|10000] ""rpm""  Receiver
 SG_ VCFRONT_pumpPowertrainRPMActual m0 : 13|10@1+ (10,0) [0|10000] ""rpm""  Receiver
 SG_ VCFRONT_radiatorFanRPMActual m0 : 24|10@1+ (10,0) [0|10000] ""rpm""  Receiver
 SG_ VCFRONT_recircExvFlow m3 : 24|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_recircExvFlowTarget m5 : 24|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ VCFRONT_recircExvState m6 : 12|4@1+ (1,0) [0|9] """"  Receiver
 SG_ VCFRONT_solenoidEvapState m2 : 37|2@1+ (1,0) [0|3] """"  Receiver
 SG_ VCFRONT_tempRefrigDischarge m2 : 3|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_tempRefrigLiquid m1 : 24|11@1+ (0.125,-40) [-40|150] ""C""  Receiver
 SG_ VCFRONT_tempSuperheatActFiltered m1 : 48|8@1- (0.25,26.5) [-5|58] ""C""  Receiver
 SG_ VCFRONT_tempSuperheatActual m0 : 34|10@1+ (0.1,-20) [-20|80] ""C""  Receiver
 SG_ VCFRONT_tempSuperheatTarget m0 : 44|10@1+ (0.1,0) [0|60] ""C""  Receiver

BO_ 984 ID3D8Elevation: 2 VehicleBus
 SG_ Elevation3D8 : 0|14@1- (1,0) [-8192|8191] ""M""  Receiver

BO_ 609 ID261_12vBattStatus: 8 VehicleBus
 SG_ VCFRONT_12VBatteryStatusChecksum : 56|8@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_12VBatteryStatusCounter : 52|4@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_LVBatteryDisconnected m1 : 50|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_LVBatterySupported m1 : 49|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_voltageFloorReachedCount m0 : 48|4@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_reverseBatteryFault m1 : 48|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_LVLoadRequest : 47|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_good12VforUpdate : 46|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_firstChargePOR m0 : 45|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_chargeNeeded m1 : 45|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_firstChargeOTA m0 : 44|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_PCSMia m1 : 44|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_silentWakeIBSCurrent m0 : 32|12@1- (0.1,-204.7) [-409.5|2.8421709430404E-014] ""mA""  Receiver
 SG_ v12vBattVoltage261 m1 : 32|12@1+ (0.00544368,0) [0|22.2918696] ""V""  Receiver
 SG_ VCFRONT_12VBatteryTargetVoltage m2 : 32|10@1+ (0.01,9) [9|19.23] ""V""  Receiver
 SG_ v12vBattCurrent261 m0 : 16|16@1- (0.005,0) [-163.84|163.835] ""A""  Receiver
 SG_ VCFRONT_IBSCurrent m1 : 16|16@1- (0.005,0) [-163.84|163.835] ""A""  Receiver
 SG_ v12vBattTemp261 m2 : 16|16@1- (0.01,0) [-327.68|327.67] ""C""  Receiver
 SG_ VCFRONT_shortedCellFaultCounter m0 : 13|3@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_isVehicleSupported m0 : 12|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_targetCurrent m1 : 8|8@1+ (0.125,0) [0|31.875] ""A""  Receiver
 SG_ VCFRONT_voltageDropCounter m0 : 8|4@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_batterySMState m1 : 4|4@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_12VOverchargeCounter m0 : 4|4@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_batterySupportRequest m1 : 3|1@1+ (1,0) [0|0] """"  Receiver
 SG_ v12vBattAH261 m2 : 2|14@1- (0.005,0) [-40.96|40.955] ""Ah""  Receiver
 SG_ VCFRONT_voltageProfile m0 : 2|2@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_IBSFault m1 : 2|1@1+ (1,0) [0|0] """"  Receiver
 SG_ VCFRONT_12VBatteryStatusIndex M : 0|2@1+ (1,0) [0|0] """"  Receiver

BO_ 297 ID129SteeringAngle: 8 VehicleBus
 SG_ SteeringSensorC129 : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ SteeringSensorB129 : 48|8@1+ (1,0) [0|255] """"  Receiver
 SG_ SteeringSensorA129 : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ SteeringSpeed129 : 32|14@1+ (0.5,-4096) [-4096|4095.5] ""D/S""  Receiver
 SG_ SteeringAngle129 : 16|14@1+ (0.1,-819.2) [-819.2|819.1] ""Deg""  Receiver

BO_ 612 ID264ChargeLineStatus: 8 VehicleBus
 SG_ ChargeLinePower264 : 24|8@1+ (0.1,0) [0|25.5] ""kW""  Receiver
 SG_ ChargeLineCurrentLimit264 : 32|10@1+ (0.1,0) [0|102.3] ""A""  Receiver
 SG_ ChargeLineVoltage264 : 0|14@1+ (0.0333,0) [0|545.554] ""V""  Receiver
 SG_ ChargeLineCurrent264 : 14|9@1+ (0.1,0) [0|51.1] ""A""  Receiver

BO_ 548 ID224PCSDCDCstatus: 8 VehicleBus
 SG_ DCDC12VSupportRtyCnt224 : 44|4@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDC12VSupportStatus224 : 2|2@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCDischargeRtyCnt224 : 48|4@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCFaulted224 : 15|1@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCHvBusDischargeStatus224 : 4|2@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCInitialPrechargeSubState224 : 59|5@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCoutputCurrent224 : 16|12@1+ (0.1,0) [0|409.5] ""A""  Receiver
 SG_ DCDCstate224 : 6|4@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCoutputCurrentMax224 : 29|12@1+ (0.1,0) [0|409.5] ""A""  Receiver
 SG_ DCDCOutputIsLimited224 : 28|1@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCPrechargeRestartCnt224 : 56|3@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCPrechargeRtyCnt224 : 41|3@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCPrechargeStatus224 : 0|2@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCPwmEnableLine224 : 52|1@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCSubState224 : 10|5@1+ (1,0) [0|0] """"  Receiver
 SG_ DCDCSupportingFixedLvTarget224 : 53|1@1+ (1,0) [0|0] """"  Receiver
 SG_ PCS_ecuLogUploadRequest224 : 54|2@1+ (1,0) [0|0] """"  Receiver

BO_ 551 ID227CMP_state: 8 VehicleBus
 SG_ CMP_HVOverVoltage : 40|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_HVUnderVoltage : 41|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_VCFRONTCANTimeout : 44|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_currentSensorCal : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_failedStart : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_inverterTemperature : 32|8@1+ (1,-40) [-40|200] ""C""  Receiver
 SG_ CMP_motorVoltageSat : 48|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_overCurrent : 45|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_overTemperature : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_ready : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_repeatOverCurrent : 50|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_shortCircuit : 49|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMP_speedDuty : 16|16@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ CMP_speedRPM : 0|16@1+ (1,0) [0|65534] ""RPM""  Receiver
 SG_ CMP_state : 56|4@1+ (1,0) [0|15] """"  Receiver
 SG_ CMP_underTemperature : 43|1@1+ (1,0) [0|1] """"  Receiver

BO_ 280 ID118DriveSystemStatus: 8 VehicleBus
 SG_ DI_accelPedalPos : 32|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DI_brakePedalState : 19|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DI_driveBlocked : 12|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DI_epbRequest : 44|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DI_gear : 21|3@1+ (1,0) [0|7] """"  Receiver
 SG_ DI_immobilizerState : 27|3@1+ (1,0) [0|6] """"  Receiver
 SG_ DI_keepDrivePowerStateRequest : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_proximity : 46|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_regenLight : 26|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_systemState : 16|3@1+ (1,0) [0|5] """"  Receiver
 SG_ DI_systemStatusChecksum : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DI_systemStatusCounter : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DI_trackModeState : 48|2@1+ (1,0) [0|2] """"  Receiver
 SG_ DI_tractionControlMode : 40|3@1+ (1,0) [0|6] """"  Receiver

BO_ 850 ID352BMS_energyStatus: 8 VehicleBus
 SG_ BMS_energyBuffer : 55|8@1+ (0.1,0) [0|25.4] ""KWh""  Receiver
 SG_ BMS_energyToChargeComplete : 44|11@1+ (0.1,0) [0|204.6] ""KWh""  Receiver
 SG_ BMS_expectedEnergyRemaining : 22|11@1+ (0.1,0) [0|204.6] ""KWh""  Receiver
 SG_ BMS_fullChargeComplete : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_idealEnergyRemaining : 33|11@1+ (0.1,0) [0|204.6] ""KWh""  Receiver
 SG_ BMS_nominalEnergyRemaining : 11|11@1+ (0.1,0) [0|204.6] ""KWh""  Receiver
 SG_ BMS_nominalFullPackEnergy : 0|11@1+ (0.1,0) [0|204.6] ""KWh""  Receiver

BO_ 893 ID37DCP_thermalStatus: 7 VehicleBus
 SG_ CP_acPinTemperature : 8|8@1+ (0.803922,-55) [-55|149.99] ""C""  Receiver
 SG_ CP_dTdt_dcPinActual : 16|12@1+ (0.005,-10) [-10|10] ""C""  Receiver
 SG_ CP_dTdt_dcPinExpected : 40|12@1+ (0.005,-10) [-10|10] ""C""  Receiver
 SG_ CP_dcPinTemperature : 0|8@1+ (0.803922,-55) [-55|149.99] ""C""  Receiver
 SG_ CP_dcPinTemperatureModeled : 32|8@1+ (0.803922,-55) [-55|149.99] ""C""  Receiver

BO_ 914 ID392BMS_packConfig: 8 VehicleBus
 SG_ BMS_packConfigMultiplexer M : 0|8@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_moduleType m1 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ BMS_packMass m1 : 16|8@1+ (1,300) [342|469] ""kg""  Receiver
 SG_ BMS_reservedConfig_0 m0 : 8|5@1+ (1,0) [0|31] """"  Receiver
 SG_ BMS_platformMaxBusVoltage m1 : 24|10@1+ (0.1,375) [0|0] ""V""  Receiver

BO_ 594 ID252BMS_powerAvailable: 8 VehicleBus
 SG_ BMS_hvacPowerBudget : 50|10@1+ (0.02,0) [0|20.46] ""kW""  Receiver
 SG_ BMS_maxDischargePower : 16|16@1+ (0.013,0) [0|655.35] ""kW""  Receiver
 SG_ BMS_maxRegenPower : 0|16@1+ (0.01,0) [0|655.35] ""kW""  Receiver
 SG_ BMS_maxStationaryHeatPower : 32|10@1+ (0.01,0) [0|10.23] ""kW""  Receiver
 SG_ BMS_notEnoughPowerForHeatPump : 42|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMS_powerLimitsState : 48|1@1+ (1,0) [0|1] """"  Receiver

BO_ 786 ID312BMSthermal: 8 VehicleBus
 SG_ BMSdissipation312 : 0|10@1+ (0.02,0) [0|20] ""kW""  Receiver
 SG_ BMSflowRequest312 : 10|7@1+ (0.3,0) [0|30] ""LPM""  Receiver
 SG_ BMSinletActiveCoolTarget312 : 17|9@1+ (0.25,-25) [-25|100] ""C""  Receiver
 SG_ BMSinletActiveHeatTarget312 : 35|9@1+ (0.25,-25) [-25|100] ""C""  Receiver
 SG_ BMSinletPassiveTarget312 : 26|9@1+ (0.25,-25) [-25|100] ""C""  Receiver
 SG_ BMSnoFlowRequest312 : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMSpcsNoFlowRequest312 : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ BMSmaxPackTemperature : 53|9@1+ (0.25,-25) [-25|100] ""C""  Receiver
 SG_ BMSminPackTemperature : 44|9@1+ (0.25,-25) [-25|100] ""C""  Receiver

BO_ 658 ID292BMS_SOC: 8 VehicleBus
 SG_ BattBeginningOfLifeEnergy292 : 40|10@1+ (0.1,0) [0|102.3] ""kWh""  Receiver
 SG_ SOCmax292 : 20|10@1+ (0.1,0) [0|102.3] ""%""  Receiver
 SG_ SOCave292 : 30|10@1+ (0.1,0) [0|102.3] ""%""  Receiver
 SG_ SOCUI292 : 10|10@1+ (0.1,0) [0|102.3] ""%""  Receiver
 SG_ SOCmin292 : 0|10@1+ (0.1,0) [0|102.3] ""%""  Receiver
 SG_ BMS_battTempPct : 50|8@1+ (0.4,0) [0|100] ""%""  Receiver

BO_ 599 ID257DIspeed: 8 VehicleBus
 SG_ DI_speedChecksum : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DI_speedCounter : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DI_uiSpeed : 24|9@1+ (1,0) [0|510] """"  Receiver
 SG_ DI_uiSpeedHighSpeed : 34|9@1+ (1,0) [0|510] """"  Receiver
 SG_ DI_uiSpeedUnits : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DI_vehicleSpeed : 12|12@1+ (0.08,-40) [-40|285] ""kph""  Receiver

BO_ 680 ID2A8CMPD_state: 8 VehicleBus
 SG_ CMPD_inputHVCurrent : 32|9@1+ (0.1,0) [0|50] ""A""  Receiver
 SG_ CMPD_inputHVPower : 21|11@1+ (10,0) [0|20000] ""W""  Receiver
 SG_ CMPD_inputHVVoltage : 41|11@1+ (0.5,0) [0|1000] ""V""  Receiver
 SG_ CMPD_powerLimitActive : 55|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMPD_powerLimitTooLowToStart : 62|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMPD_ready : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ CMPD_speedDuty : 11|10@1+ (0.1,0) [0|100] ""%""  Receiver
 SG_ CMPD_speedRPM : 0|11@1+ (10,0) [0|20000] ""RPM""  Receiver
 SG_ CMPD_state : 56|4@1+ (1,0) [0|15] """"  Receiver
 SG_ CMPD_wasteHeatState : 60|2@1+ (1,0) [0|3] """"  Receiver

BO_ 1029 ID405VIN: 8 VehicleBus
 SG_ VINB405 m17 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ VINC405 m18 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ VINA405 m16 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ mux405 M : 0|8@1+ (1,0) [0|255] """"  Receiver

BO_ 1310 ID51EFC_info: 8 VehicleBus
 SG_ FC_infoIndex M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoAppGitHashBytes m17 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ FC_infoApplicationCRC m13 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ FC_infoAssemblyID m11 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoBootCRC m15 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ FC_infoBootGitHashBytes m18 : 8|56@1+ (1,0) [0|7.20576E+016] """"  Receiver
 SG_ FC_infoBootSvnRev m14 : 8|24@1+ (1,0) [0|16777200] """"  Receiver
 SG_ FC_infoBootUdsProtoVersion m20 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoBranchOrigin m19 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoBuildConfigID m10 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoBuildType m10 : 8|3@1+ (1,0) [0|4] """"  Receiver
 SG_ FC_infoCPLDVersionMajor m16 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoCPLDVersionMinor m16 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoComponentID m10 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ FC_infoHardwareID m10 : 32|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ FC_infoHardwareRevision m19 : 40|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoMajorVersion m19 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoMaturity m19 : 32|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoPcbaID m11 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoPlatformType m19 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ FC_infoSubUsageID m11 : 48|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ FC_infoUsageID m11 : 32|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ FC_infoVariantCRC m22 : 32|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ FC_partNumChar01 m25 : 8|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar02 m25 : 16|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar03 m25 : 24|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar04 m25 : 32|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar05 m25 : 40|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar06 m25 : 48|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar07 m25 : 56|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar08 m26 : 8|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar09 m26 : 16|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar10 m26 : 24|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar11 m26 : 32|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar12 m26 : 40|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar13 m26 : 48|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar14 m26 : 56|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar15 m27 : 8|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar16 m27 : 16|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar17 m27 : 24|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar18 m27 : 32|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar19 m27 : 40|8@1+ (1,0) [0|1] """"  Receiver
 SG_ FC_partNumChar20 m27 : 48|8@1+ (1,0) [0|1] """"  Receiver

BO_ 886 ID376FrontInverterTemps: 8 VehicleBus
 SG_ TempInvPCB376 : 0|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ TempPctStator376 : 48|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ TempPctInverter376 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ TempInvCapbank376 : 24|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ TempStator376 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ TempInvHeatsink376 : 32|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ TempInverter376 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIF_inverterTQF : 56|2@1+ (1,0) [0|3] """"  Receiver

BO_ 789 ID315RearInverterTemps: 8 VehicleBus
 SG_ RearTempInvPCB315 : 0|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ RearTempPctStator315 : 48|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ RearTempPctInverter315 : 40|8@1+ (0.4,0) [0|102] ""%""  Receiver
 SG_ RearTempInvCapbank315 : 24|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ RearTempStator315 : 16|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ RearTempInvHeatsink315 : 32|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ RearTempInverter315 : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ DIR_inverterTQF : 56|2@1+ (1,0) [0|3] """"  Receiver

BO_ 340 ID154RearTorqueOld: 8 VehicleBus
 SG_ RAWTorqueRear154 : 40|12@1- (0.25,0) [-512|511.75] ""NM""  Receiver

BO_ 950 ID3B6odometer: 4 VehicleBus
 SG_ Odometer3B6 : 0|32@1+ (0.001,0) [0|4294970] ""KM""  Receiver

BO_ 614 ID266RearInverterPower: 8 VehicleBus
 SG_ RearHeatPowerMax266 : 24|8@1+ (0.08,0) [0|20] ""kW""  Receiver
 SG_ RearPowerLimit266 : 48|9@1+ (1,0) [0|400] ""kW""  Receiver
 SG_ RearHeatPower266 : 32|8@1+ (0.08,0) [0|20] ""kW""  Receiver
 SG_ RearHeatPowerOptimal266 : 16|8@1+ (0.08,0) [0|20] ""kW""  Receiver
 SG_ RearExcessHeatCmd : 40|8@1+ (0.08,0) [0|20] ""kW""  Receiver
 SG_ RearPower266 : 0|11@1- (0.5,0) [-500|500] ""kW""  Receiver

BO_ 741 ID2E5FrontInverterPower: 8 VehicleBus
 SG_ FrontHeatPowerOptimal2E5 : 16|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ FrontPower2E5 : 0|11@1- (0.5,0) [-512|511.5] ""kW""  Receiver
 SG_ FrontHeatPowerMax2E5 : 24|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ FrontPowerLimit2E5 : 48|9@1+ (1,0) [0|400] ""kW""  Receiver
 SG_ FrontHeatPower2E5 : 32|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ FrontExcessHeatCmd : 40|8@1+ (0.08,0) [0|20] ""kW""  Receiver

BO_ 742 ID2E6PlaidFrontPower: 8 VehicleBus
 SG_ PFrontHeatPowerOptimal : 16|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ PFrontPower : 0|11@1- (0.5,0) [-512|511.5] ""kW""  Receiver
 SG_ PFrontHeatPowerMax : 24|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ PFrontPowerLimit : 48|9@1+ (1,0) [0|400] ""kW""  Receiver
 SG_ PFrontHeatPower : 32|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ PFrontExcessHeatCmd : 40|8@1+ (0.08,0) [0|20] ""kW""  Receiver

BO_ 617 ID269LeftRearPower: 8 VehicleBus
 SG_ LeftRearHeatPowerOptimal : 16|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ LeftRearPower : 0|11@1- (0.5,0) [-512|511.5] ""kW""  Receiver
 SG_ LeftRearPowerMax : 24|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ LeftRearPowerLimit : 48|9@1+ (1,0) [0|400] ""kW""  Receiver
 SG_ LeftRearHeatPower : 32|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ LeftRearExcessHeatCmd : 40|8@1+ (0.08,0) [0|20] ""kW""  Receiver

BO_ 636 ID27CRightRearPower: 8 VehicleBus
 SG_ RightRearHeatPowerOptimal : 16|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ RightRearPower : 0|11@1- (0.5,0) [-512|511.5] ""kW""  Receiver
 SG_ RightRearHeatPowerMax : 24|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ RightRearPowerLimit : 48|9@1+ (1,0) [0|400] ""kW""  Receiver
 SG_ RightRearHeatPower : 32|8@1+ (0.08,0) [0|20.4] ""kW""  Receiver
 SG_ RightRearExcessHeatCmd : 40|8@1+ (0.08,0) [0|20] ""kW""  Receiver

BO_ 264 ID108DIR_torque: 8 VehicleBus
 SG_ DIR_axleSpeed : 40|16@1- (0.1,0) [-2750|2750] ""RPM""  Receiver
 SG_ DIR_slavePedalPos : 56|8@1+ (0.4,0) [0|100] ""%""  Receiver
 SG_ DIR_torqueActual : 27|13@1- (2,0) [-7500|7500] ""Nm""  Receiver
 SG_ DIR_torqueChecksum : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ DIR_torqueCommand : 12|13@1- (2,0) [-7500|7500] ""Nm""  Receiver
 SG_ DIR_torqueCounter : 8|4@1+ (1,0) [0|15] """"  Receiver
 SG_ DIR_axleSpeedQF : 25|1@1+ (1,0) [0|1] """"  Receiver

BO_ 306 ID132HVBattAmpVolt: 8 VehicleBus
 SG_ ChargeHoursRemaining132 : 48|12@1+ (1,0) [0|4095] ""Min""  Receiver
 SG_ BattVoltage132 : 0|16@1+ (0.01,0) [0|655.35] ""V""  Receiver
 SG_ RawBattCurrent132 : 32|16@1- (-0.05,822) [-1138.35|2138.4] ""A""  Receiver
 SG_ SmoothBattCurrent132 : 16|16@1- (-0.1,0) [-3276.7|3276.7] ""A""  Receiver

BO_ 294 ID126RearHVStatus: 7 VehicleBus
 SG_ RearHighVoltage126 : 0|10@1+ (0.5,0) [0|500] ""V""  Receiver
 SG_ RearMotorCurrent126 : 11|11@1+ (1,0) [0|2047] ""A""  Receiver
 SG_ DIR_switchingFrequency : 40|11@1+ (0.01,0) [0|20] ""kHz""  Receiver
 SG_ DIR_dcCableCurrentEst : 24|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIR_vBatQF : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ DIR_targetFluxMode : 51|2@1+ (1,0) [0|0] """"  Receiver

BO_ 421 ID1A5FrontHVStatus: 7 VehicleBus
 SG_ FrontHighVoltage1A5 : 0|10@1+ (0.5,0) [0|500] ""V""  Receiver
 SG_ FrontMotorCurrent1A5 : 11|11@1+ (1,0) [0|2047] ""A""  Receiver
 SG_ DIF_switchingFrequency : 40|11@1+ (0.01,0) [0|20] ""kHz""  Receiver
 SG_ DIF_dcCableCurrentEst : 24|16@1- (0.1,0) [-3276.8|3276.7] ""A""  Receiver
 SG_ DIF_vBatQF : 10|1@1+ (1,0) [0|0] """"  Receiver
 SG_ DIF_targetFluxMode : 51|2@1+ (1,0) [0|0] """"  Receiver

BO_ 295 ID127LeftRearHVStatus: 7 VehicleBus
 SG_ LeftRear_targetFluxMode : 51|2@1+ (1,0) [0|0] """"  Receiver
 SG_ LeftRear_switchingFrequency : 40|11@1+ (0.01,0) [0|0] ""kHz""  Receiver
 SG_ LeftRear_dcCableCurrentEst : 24|16@1- (0.1,0) [0|0] ""A""  Receiver
 SG_ LeftRear_motorCurrent : 11|11@1+ (1,0) [0|0] ""A""  Receiver
 SG_ LeftRear_vBatQF : 10|1@1+ (1,0) [0|0] """"  Receiver
 SG_ LeftRear_vBat : 0|10@1+ (0.5,0) [0|0] ""V""  Receiver

BO_ 298 ID12ARightRearHVStatus: 7 VehicleBus
 SG_ RightRear_targetFluxMode : 51|2@1+ (1,0) [0|0] """"  Receiver
 SG_ RightRear_switchingFrequency : 40|11@1+ (0.01,0) [0|0] ""kHz""  Receiver
 SG_ RightRear_dcCableCurrentEst : 24|16@1- (0.1,0) [0|0] ""A""  Receiver
 SG_ RightRear_motorCurrent : 11|11@1+ (1,0) [0|0] ""A""  Receiver
 SG_ RightRear_vBatQF : 10|1@1+ (1,0) [0|0] """"  Receiver
 SG_ RightRear_vBat : 0|10@1+ (0.5,0) [0|0] ""V""  Receiver

BO_ 799 ID31FTPMSsensors: 8 ChassisBus
 SG_ TPMSFLpressure31F : 0|8@1+ (0.025,0) [0|6.375] ""bar""  Receiver
 SG_ TPMSFRpressure31F : 16|8@1+ (0.025,0) [0|6.375] ""bar""  Receiver
 SG_ TPMSRLpressure31F : 32|8@1+ (0.025,0) [0|6.375] ""bar""  Receiver
 SG_ TPMSRRpressure31F : 48|8@1+ (0.025,0) [0|6.375] ""bar""  Receiver
 SG_ TPMSFLtemp31F : 8|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ TPMSFRtemp31F : 24|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ TPMSRLtemp31F : 40|8@1+ (1,-40) [-40|215] ""C""  Receiver
 SG_ TPMSRRtemp31F : 56|8@1+ (1,-40) [-40|215] ""C""  Receiver

BO_ 1022 ID3FEbrakeTemps: 5 VehicleBus
 SG_ BrakeTempFL3FE : 0|10@1+ (1,-40) [0|0] ""C""  Receiver
 SG_ BrakeTempFR3FE : 10|10@1+ (1,-40) [0|0] ""C""  Receiver
 SG_ BrakeTempRL3FE : 20|10@1+ (1,-40) [0|0] ""C""  Receiver
 SG_ BrakeTempRR3FE : 30|10@1+ (1,-40) [0|0] ""C""  Receiver

BO_ 552 ID228EPBrightStatus: 8 VehicleBus
 SG_ EPBR12VFilt228 : 37|12@1+ (0.00544368,0) [0|0] ""V""  Receiver
 SG_ EPBRcsmFaultReason228 : 21|5@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRdisconnected228 : 10|1@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRCDPQualified228 : 49|2@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBResmCaliperRequest228 : 18|3@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBResmOperationTrigger228 : 26|5@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRinternalCDPRequest228 : 51|1@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRinternalStatusChecksum228 : 56|8@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRinternalStatusCounter228 : 52|4@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRlocalServiceModeActive228 : 36|1@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRlockoutUnlockCount228 : 11|7@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRsummonFaultReason228 : 31|5@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRsummonState228 : 7|3@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRunitFaultStatus228 : 5|2@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBRunitStatus228 : 0|5@1+ (1,0) [0|0] """"  Receiver

BO_ 648 ID288EPBleftStatus: 8 VehicleBus
 SG_ EPBL12VFilt288 : 37|12@1+ (0.00544368,0) [0|0] ""V""  Receiver
 SG_ EPBLcsmFaultReason288 : 21|5@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLdisconnected288 : 10|1@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLCDPQualified288 : 49|2@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLesmCaliperRequest288 : 18|3@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLesmOperationTrigger288 : 26|5@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLinternalCDPRequest288 : 51|1@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLinternalStatusChecksum288 : 56|8@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLinternalStatusCounter288 : 52|4@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLlocalServiceModeActive288 : 36|1@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLlockoutUnlockCount288 : 11|7@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLsummonFaultReason288 : 31|5@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLsummonState288 : 7|3@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLunitFaultStatus288 : 5|2@1+ (1,0) [0|0] """"  Receiver
 SG_ EPBLunitStatus288 : 0|5@1+ (1,0) [0|0] """"  Receiver

BO_ 1834 ID72ABMS_serialNumber: 8 VehicleBus
 SG_ BMS_packSerialNumberByte01 m0 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte02 m0 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte03 m0 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte04 m0 : 32|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte05 m0 : 40|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte06 m0 : 48|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte07 m0 : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte08 m1 : 8|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte09 m1 : 16|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte10 m1 : 24|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte11 m1 : 32|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte12 m1 : 40|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte13 m1 : 48|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_packSerialNumberByte14 m1 : 56|8@1+ (1,0) [0|255] """"  Receiver
 SG_ BMS_serialNumberMultiplexer M : 0|1@1+ (1,0) [0|1] """"  Receiver

BO_ 2047 ID7FFcarConfig: 8 VehicleBus
 SG_ GTW_carConfigMultiplexer M : 0|8@1+ (1,0) [0|255] """"  Receiver
 SG_ GTW_activeHighBeam m2 : 34|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_airSuspension m3 : 22|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_airbagCutoffSwitch m2 : 35|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_audioType m3 : 30|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_autopilot m2 : 42|3@1+ (1,0) [0|4] """"  Receiver
 SG_ GTW_autopilotCameraType m3 : 26|1@1+ (1,0) [0|0] """"  Receiver
 SG_ GTW_auxParkLamps m2 : 26|2@1+ (1,0) [0|3] """"  Receiver
 SG_ GTW_bPillarNFCParam m2 : 56|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_birthday m4 : 8|32@1+ (1,0) [0|4294970000] """"  Receiver
 SG_ GTW_brakeHWType m2 : 59|2@1+ (1,0) [0|3] """"  Receiver
 SG_ GTW_brakeLineSwitchType m3 : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_cabinPTCHeaterType m2 : 31|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_chassisType m3 : 18|3@1+ (1,0) [0|4] """"  Receiver
 SG_ GTW_compressorType m4 : 46|2@1+ (1,0) [0|3] """"  Receiver
 SG_ GTW_connectivityPackage m3 : 27|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_coolantPumpType m3 : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_country m1 : 16|16@1+ (1,0) [0|65535] """"  Receiver
 SG_ GTW_dasHw m1 : 40|3@1+ (1,0) [3|4] """"  Receiver
 SG_ GTW_deliveryStatus m1 : 8|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_drivetrainType m1 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_eBuckConfig m2 : 32|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_eCallEnabled m4 : 40|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_efficiencyPackage m4 : 48|3@1+ (1,0) [0|5] """"  Receiver
 SG_ GTW_epasType m1 : 9|1@1+ (1,0) [0|0] """"  Receiver
 SG_ GTW_espValveType m3 : 40|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_exteriorColor m2 : 48|3@1+ (1,0) [0|6] """"  Receiver
 SG_ GTW_frontFogLamps m2 : 20|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_frontSeatHeaters m2 : 9|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_frontSeatReclinerHardware m3 : 37|2@1+ (1,0) [0|3] """"  Receiver
 SG_ GTW_frontSeatType m3 : 60|3@1+ (1,0) [0|4] """"  Receiver
 SG_ GTW_headlamps m1 : 14|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_headlightLevelerType m3 : 47|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_homelinkType m2 : 13|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_hvacPanelVaneType m2 : 29|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_immersiveAudio m3 : 56|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_interiorLighting m2 : 57|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_intrusionSensorType m2 : 36|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_lumbarECUType m2 : 23|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_mapRegion m3 : 8|4@1+ (1,0) [0|10] """"  Receiver
 SG_ GTW_memoryMirrors m2 : 17|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_numberHVILNodes m2 : 51|2@1+ (1,0) [2|3] """"  Receiver
 SG_ GTW_packEnergy m3 : 32|5@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_passengerAirbagType m4 : 43|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_passengerOccupancySensorType m3 : 24|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_pedestrianWarningSound m2 : 54|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_performancePackage m3 : 12|3@1+ (1,0) [0|3] """"  Receiver
 SG_ GTW_plcSupportType m3 : 28|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_powerSteeringColumn m2 : 18|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_radarHeaterType m3 : 55|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_rearFogLamps m2 : 39|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_rearGlassType m2 : 38|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_rearLightType m1 : 12|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_rearSeatHeaters m2 : 10|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_refrigerantType m3 : 45|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_restraintsHardwareType m1 : 48|8@1+ (1,0) [21|163] """"  Receiver
 SG_ GTW_rightHandDrive m1 : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_roofGlassType m2 : 61|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_roofType m2 : 40|1@1+ (1,0) [1|1] """"  Receiver
 SG_ GTW_softRange m3 : 42|3@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_spoilerType m2 : 37|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_steeringColumnMotorType m4 : 52|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_steeringColumnUJointType m2 : 55|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_superchargingAccess m2 : 45|2@1+ (1,0) [0|2] """"  Receiver
 SG_ GTW_tireType m1 : 32|5@1+ (1,0) [0|21] """"  Receiver
 SG_ GTW_towPackage m3 : 15|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_tpmsType m2 : 11|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_twelveVBatteryType m3 : 63|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_vdcType m2 : 14|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_wheelType m3 : 48|7@1+ (1,0) [0|20] """"  Receiver
 SG_ GTW_windshieldType m2 : 33|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_xcpESP m2 : 16|1@1+ (1,0) [0|1] """"  Receiver
 SG_ GTW_xcpIbst m2 : 15|1@1+ (1,0) [0|1] """"  Receiver

BO_ 818 ID332BattBrickMinMax: 6 VehicleBus
 SG_ BattBrickMultiplexer332 M : 0|2@1+ (1,0) [0|0] """"  Receiver
 SG_ BattBrickempMaxNum332 m0 : 2|4@1+ (1,0) [0|0] """"  Receiver
 SG_ BattBrickTempMinNum332 m0 : 8|4@1+ (1,0) [0|0] """"  Receiver
 SG_ BattBrickTempMax332 m0 : 16|8@1+ (0.5,-40) [0|0] ""C""  Receiver
 SG_ BattBrickTempMin332 m0 : 24|8@1+ (0.5,-40) [0|0] ""C""  Receiver
 SG_ BattBrickModelTMax332 m0 : 32|8@1+ (0.5,-40) [0|0] ""C""  Receiver
 SG_ BattBrickModelTMin332 m0 : 40|8@1+ (0.5,-40) [0|0] ""C""  Receiver
 SG_ BattBrickVoltageMax332 m1 : 2|12@1+ (0.002,0) [0|0] ""V""  Receiver
 SG_ BattBrickVoltageMin332 m1 : 16|12@1+ (0.002,0) [0|0] ""V""  Receiver
 SG_ BattBrickVoltageMaxNum332 m1 : 32|7@1+ (1,1) [0|0] """"  Receiver
 SG_ BattBrickVoltageMinNum332 m1 : 40|7@1+ (1,1) [0|0] """"  Receiver

BO_ 1025 ID401BrickVoltages: 8 VehicleBus
 SG_ MultiplexSelector M : 0|8@1+ (1,0) [0|0] """"  Receiver
 SG_ StatusFlags : 8|8@1+ (1,0) [0|0] """"  Receiver
 SG_ Brick0 m0 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick1 m0 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick2 m0 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick3 m1 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick4 m1 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick5 m1 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick6 m2 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick7 m2 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick8 m2 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick9 m3 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick10 m3 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick11 m3 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick12 m4 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick13 m4 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick14 m4 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick15 m5 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick16 m5 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick17 m5 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick18 m6 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick19 m6 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick20 m6 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick21 m7 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick22 m7 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick23 m7 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick24 m8 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick25 m8 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick26 m8 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick27 m9 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick28 m9 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick29 m9 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick30 m10 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick31 m10 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick32 m10 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick34 m11 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick33 m11 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick35 m11 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick36 m12 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick37 m12 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick38 m12 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick39 m13 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick40 m13 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick41 m13 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick42 m14 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick43 m14 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick44 m14 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick45 m15 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick46 m15 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick47 m15 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick48 m16 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick49 m16 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick50 m16 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick51 m17 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick52 m17 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick53 m17 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick54 m18 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick55 m18 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick56 m18 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick57 m19 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick58 m19 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick59 m19 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick60 m20 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick61 m20 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick62 m20 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick63 m21 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick64 m21 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick65 m21 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick66 m22 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick67 m22 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick68 m22 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick69 m23 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick70 m23 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick71 m23 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick72 m24 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick73 m24 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick74 m24 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick75 m25 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick76 m25 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick77 m25 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick78 m26 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick79 m26 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick80 m26 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick81 m27 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick82 m27 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick83 m27 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick84 m28 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick85 m28 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick86 m28 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick87 m29 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick88 m29 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick89 m29 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick90 m30 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick91 m30 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick92 m30 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick93 m31 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick94 m31 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick95 m31 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick96 m32 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick97 m32 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick98 m32 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick99 m33 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick100 m33 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick101 m33 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick102 m34 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick103 m34 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick104 m34 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick105 m35 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick106 m35 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick107 m35 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick108 m36 : 16|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick109 m36 : 32|16@1+ (0.0001,0) [0|0] ""V""  Receiver
 SG_ Brick110 m36 : 48|16@1+ (0.0001,0) [0|0] ""V""  Receiver

";
await using var dbcStream = File.OpenRead("./database.dbc");
using var dbcReader = new StringReader(testString);
dbc.AddFile(dbcReader);

var logger = loggerFactory.CreateLogger<Program>();
var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
var files = dir.EnumerateFiles("*.log");
foreach (var file in files)
{
    await using var fs = file.OpenRead();
    var reader = await Reader.Open(fs, loggerFactory.CreateLogger<Reader>());
    var frames = 0;
    while (true)
    {
        var frame = reader.GetNextFrame();
        frames++;
        if (frames % 1000 == 0)
            logger.LogInformation("Processed {frames} frames", frames);
        if (frame == null)
            return;
    }
}