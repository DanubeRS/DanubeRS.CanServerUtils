﻿// See https://aka.ms/new-console-template for more information

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

CM_ SG_ 130 UI_tripPlanningActive ""Navigation Active"";
CM_ SG_ 257 RCM_pitchRate ""Pitch"";
CM_ SG_ 257 RCM_rollRate ""Roll"";
CM_ SG_ 257 RCM_yawRate ""Yaw"";
CM_ SG_ 273 RCM_lateralAccel ""L/R Acceleration"";
CM_ SG_ 273 RCM_longitudinalAccel ""F/B Acceleration"";
CM_ SG_ 273 RCM_verticalAccel ""Vertical Acceleration"";
CM_ SG_ 692 PCS_dcdcHvBusVolt ""DCDC HV Voltage"";
CM_ SG_ 692 PCS_dcdcLvBusVolt ""DCDC 12v Voltage"";
CM_ SG_ 692 PCS_dcdcLvOutputCurrent ""DCDC 12v Output Current"";
CM_ SG_ 985 UI_gpsVehicleHeading ""GPS Heading"";
CM_ SG_ 985 UI_gpsVehicleSpeed ""GPS Speed"";
CM_ SG_ 1011 UI_odometer ""UI Odometer"";
CM_ SG_ 615 DI_gradeEst ""Estimated Grade"";
CM_ SG_ 615 DI_mass ""Estimated Mass"";
CM_ SG_ 642 VCLEFT_hvacBlowerRPMActual ""HVAC Blower Speed"";
CM_ SG_ 647 PTC_leftCurrentHV ""Heater Left Current"";
CM_ SG_ 647 PTC_rightCurrentHV ""Heater Right Current"";
CM_ SG_ 647 PTC_voltageHV ""Heater Voltage"";
CM_ SG_ 819 UI_acChargeCurrentLimit ""UI Charge Current Limit"";
CM_ SG_ 826 UI_Range ""Range"";
CM_ SG_ 826 UI_SOC ""State of Charge UI"";
CM_ SG_ 826 UI_uSOE ""UI fine SOC"";
CM_ SG_ 826 UI_ratedWHpM ""WHM Rating"";
CM_ SG_ 577 VCFRONT_coolantFlowBatActual ""Battery Coolant Flow"";
CM_ SG_ 577 VCFRONT_coolantFlowPTActual ""Powertrain Coolant Flow"";
CM_ BO_ 1367 ""swapped with 5D7 in old firmware"";
CM_ BO_ 1495 ""swapped with 557 in old firmware"";
CM_ BO_ 2005 ""swapped with 757 in old firmware"";
CM_ BO_ 1879 ""swapped with 7D5 in old firmware"";
CM_ SG_ 579 VCRIGHT_hvacCabinTempEst ""Cabin Temperature"";
CM_ SG_ 524 VCRIGHT_wattsDemandEvap ""Evaporator Power"";
CM_ SG_ 792 UTCyear318 ""Year"";
CM_ SG_ 792 UTCmonth318 ""Month"";
CM_ SG_ 792 UTCseconds318 ""Seconds"";
CM_ SG_ 792 UTChour318 ""Hour"";
CM_ SG_ 792 UTCday318 ""Day"";
CM_ SG_ 792 UTCminutes318 ""Minute"";
CM_ SG_ 1320 UnixTimeSeconds528 ""Unix Time"";
CM_ SG_ 390 DIF_axleSpeed ""Front Axle Speed"";
CM_ SG_ 390 DIF_torqueActual ""Front Axle Torque"";
CM_ SG_ 390 DIF_torqueCommand ""Front Axle Torque Request"";
CM_ BO_ 918 ""swapped with 395 in old firmware"";
CM_ BO_ 917 ""swapped with 396 in old firmware"";
CM_ SG_ 472 RearTorqueRequest1D8 ""Rear Motor Torque Request"";
CM_ SG_ 472 RearTorque1D8 ""Rear Motor Torque"";
CM_ SG_ 468 RAWTorqueFront1D4 ""Front Torque Old, Axle torque 2 with 9/1 gearing"";
CM_ SG_ 469 FrontTorqueRequest1D5 ""Front Motor Torque Request"";
CM_ SG_ 469 FrontTorque1D5 ""Front Motor Torque"";
CM_ SG_ 741 FrontPower2E5 ""Front Motor Power"";
CM_ SG_ 741 FrontPowerLimit2E5 ""Front Power Limit, approx offset Orig scale 1"";
CM_ SG_ 741 FrontHeatPower2E5 ""Front Waste Heat Power"";
CM_ SG_ 822 DriveRegenRating336 ""Regen Rating"";
CM_ SG_ 822 DrivePowerRating336 ""Power Rating"";
CM_ SG_ 616 SystemRegenPowerMax268 ""Max Regen Power"";
CM_ SG_ 616 SystemHeatPowerMax268 ""System Max Waste Heat Power"";
CM_ SG_ 616 SystemHeatPower268 ""System Waste Heat Power"";
CM_ SG_ 616 SystemDrivePowerMax268 ""Max Drive Power"";
CM_ SG_ 79 GPSLongitude04F ""Longitude"";
CM_ SG_ 79 GPSLatitude04F ""Latitude"";
CM_ SG_ 978 TotalDischargeKWh3D2 ""Lifetime Discharge"";
CM_ SG_ 978 TotalChargeKWh3D2 ""Lifetime Charge"";
CM_ SG_ 722 MinVoltage2D2 ""BMS Min Voltage"";
CM_ SG_ 722 MaxDischargeCurrent2D2 ""BMS Max Discharge Current"";
CM_ SG_ 722 MaxChargeCurrent2D2 ""BMS Max Charge Current"";
CM_ SG_ 722 MaxVoltage2D2 ""BMS Max Voltage"";
CM_ SG_ 1345 FCMaxPowerLimit541 ""Supercharger Power Limit"";
CM_ SG_ 1345 FCMaxCurrentLimit541 ""Supercharger Current Limit"";
CM_ SG_ 580 FCMinVlimit244 ""Supercharger Min Voltage"";
CM_ SG_ 580 FCMaxVlimit244 ""Supercharger Max Voltage"";
CM_ SG_ 580 FCCurrentLimit244 ""Supercharger Max Current"";
CM_ SG_ 580 FCPowerLimit244 ""Supercharger Max Power"";
CM_ SG_ 801 VCFRONT_tempAmbientFiltered ""Outside Temperature"";
CM_ SG_ 801 VCFRONT_tempCoolantBatInlet ""Battery Coolant Temp"";
CM_ SG_ 801 VCFRONT_tempCoolantPTInlet ""Powertrain Coolant Temp"";
CM_ SG_ 984 Elevation3D8 ""Elevation"";
CM_ SG_ 609 v12vBattVoltage261 ""12V Battery Voltage"";
CM_ SG_ 609 v12vBattCurrent261 ""12V Battery Current"";
CM_ SG_ 609 v12vBattTemp261 ""12V Battery Temp"";
CM_ SG_ 609 v12vBattAH261 ""12V Battery Capacity"";
CM_ SG_ 297 SteeringSpeed129 ""Steering Speed"";
CM_ SG_ 297 SteeringAngle129 ""Steering Angle"";
CM_ SG_ 612 ChargeLinePower264 ""Charger Line Power"";
CM_ SG_ 612 ChargeLineCurrentLimit264 ""Charge Connector Current Limit"";
CM_ SG_ 612 ChargeLineVoltage264 ""Charger Line Voltage"";
CM_ SG_ 612 ChargeLineCurrent264 ""Charger Line Current"";
CM_ SG_ 548 DCDCoutputCurrent224 ""DCDC Output Current"";
CM_ SG_ 280 DI_accelPedalPos ""Pedal Position"";
CM_ SG_ 280 DI_brakePedalState ""Brake Pedal"";
CM_ SG_ 280 DI_gear ""Gear"";
CM_ SG_ 280 DI_regenLight ""Regen Brake"";
CM_ SG_ 280 DI_tractionControlMode ""Traction Control Mode"";
CM_ SG_ 850 BMS_energyBuffer ""Battery Buffer kWh"";
CM_ SG_ 850 BMS_energyToChargeComplete ""Charge Remaining kWh"";
CM_ SG_ 850 BMS_fullChargeComplete ""Charge Complete"";
CM_ SG_ 850 BMS_nominalEnergyRemaining ""Batt Remaining kWh"";
CM_ SG_ 850 BMS_nominalFullPackEnergy ""Batt Full kWh"";
CM_ SG_ 594 BMS_hvacPowerBudget ""BMS Max HVAC Power"";
CM_ SG_ 594 BMS_maxDischargePower ""BMS Max Discharge Power"";
CM_ SG_ 594 BMS_maxRegenPower ""BMS Max Regen Power"";
CM_ SG_ 594 BMS_maxStationaryHeatPower ""BMS Max Waste Heat Power"";
CM_ SG_ 786 BMSdissipation312 ""Battery Dissipation"";
CM_ SG_ 786 BMSinletActiveCoolTarget312 ""Battery Cool Target"";
CM_ SG_ 786 BMSinletActiveHeatTarget312 ""Battery Heat Target"";
CM_ SG_ 786 BMSmaxPackTemperature ""Max Battery Temp"";
CM_ SG_ 786 BMSminPackTemperature ""Min Battery Temp"";
CM_ SG_ 658 BattBeginningOfLifeEnergy292 ""HV Battery Original Energy"";
CM_ SG_ 658 SOCmax292 ""BMS Max SOC"";
CM_ SG_ 658 SOCave292 ""BMS Ave SOC"";
CM_ SG_ 658 SOCmin292 ""BMS Min SOC"";
CM_ SG_ 599 DI_uiSpeed ""UI Speed"";
CM_ SG_ 599 DI_uiSpeedUnits ""Speed Units, 0-mph 1-kph"";
CM_ SG_ 599 DI_vehicleSpeed ""Vehicle Speed, .05 -25 for mph"";
CM_ SG_ 1029 VINB405 ""VIN3, last 7 of VIN (ASCII)"";
CM_ SG_ 1029 VINC405 ""VIN2, part two of VIN (ASCII)"";
CM_ SG_ 1029 VINA405 ""VIN1, four zeros then first 3 characters of VIN"";
CM_ SG_ 886 TempStator376 ""Front Stator Temp"";
CM_ SG_ 789 RearTempStator315 ""Rear Stator Temp"";
CM_ SG_ 340 RAWTorqueRear154 ""Rear Torque Old, Axle torque 2 with 9/1 gearing"";
CM_ SG_ 950 Odometer3B6 ""Odometer"";
CM_ SG_ 614 RearPowerLimit266 ""Rear Power Limit, approx offset Orig scale 1"";
CM_ SG_ 614 RearHeatPower266 ""Rear Waste Heat Power"";
CM_ SG_ 614 RearPower266 ""Rear Motor Power"";
CM_ SG_ 741 FrontPower2E5 ""Front Motor Power"";
CM_ SG_ 741 FrontPowerLimit2E5 ""Front Power Limit, approx offset Orig scale 1"";
CM_ SG_ 741 FrontHeatPower2E5 ""Front Waste Heat Power"";
CM_ SG_ 264 DIR_axleSpeed ""Rear Axle Speed"";
CM_ SG_ 264 DIR_torqueActual ""Rear Axle Torque"";
CM_ SG_ 264 DIR_torqueCommand ""Rear Axle Torque Request"";
CM_ SG_ 306 ChargeHoursRemaining132 ""Charge Time Remaining"";
CM_ SG_ 306 BattVoltage132 ""HV Battery Voltage"";
CM_ SG_ 306 RawBattCurrent132 ""HV Raw Current, old offset 1000"";
CM_ SG_ 306 SmoothBattCurrent132 ""HV Battery Current"";
CM_ SG_ 294 RearHighVoltage126 ""Rear Motor Voltage"";
CM_ SG_ 294 RearMotorCurrent126 ""Rear Motor Current"";
CM_ SG_ 421 FrontHighVoltage1A5 ""Front Motor Voltage"";
CM_ SG_ 421 FrontMotorCurrent1A5 ""Front Motor Current"";
CM_ BO_ 1022 ""Brake Temps Estimated"";
CM_ SG_ 2047 GTW_chassisType ""Model"";
CM_ SG_ 2047 GTW_packEnergy ""Battery Pack Size"";
CM_ SG_ 818 BattBrickTempMax332 ""Max Batt Brick Temp"";
CM_ SG_ 818 BattBrickTempMin332 ""Min Batt Brick Temp"";
CM_ SG_ 818 BattBrickVoltageMax332 ""Max Batt Brick V"";
CM_ SG_ 818 BattBrickVoltageMin332 ""Min Batt Brick V"";
BA_DEF_ BO_  ""GenMsgSendType"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"";
BA_DEF_ BO_  ""GenMsgDelayTime"" INT 0 0;
BA_DEF_ BO_  ""GenMsgCycleTime"" INT 0 0;
BA_DEF_ BO_  ""GenMsgTimeoutTime"" INT 0 10000;
BA_DEF_  ""ProtocolType"" STRING ;
BA_DEF_  ""BusType"" STRING ;
BA_DEF_ BU_  ""NodeLayerModules"" STRING ;
BA_DEF_ BU_  ""ECU"" STRING ;
BA_DEF_ BU_  ""CANoeJitterMax"" INT 0 0;
BA_DEF_ BU_  ""CANoeJitterMin"" INT 0 0;
BA_DEF_ BU_  ""CANoeDrift"" INT 0 0;
BA_DEF_ BU_  ""CANoeStartDelay"" INT 0 0;
BA_DEF_ BO_  ""GenMsgAutoGenSnd"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"",""No"",""Yes"";
BA_DEF_ BO_  ""GenMsgAutoGenDsp"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"",""No"",""Yes"",""No"",""Yes"";
BA_DEF_ SG_  ""GenSigAutoGenSnd"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"";
BA_DEF_ SG_  ""GenSigAutoGenDsp"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"";
BA_DEF_ SG_  ""GenSigEnvVarType"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"",""int"",""float"",""undef"";
BA_DEF_ SG_  ""GenSigEVName"" STRING ;
BA_DEF_ BU_  ""GenNodAutoGenSnd"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"",""int"",""float"",""undef"",""No"",""Yes"";
BA_DEF_ BU_  ""GenNodAutoGenDsp"" ENUM  ""Cyclic"",""SendType1"",""SendType2"",""SendType3"",""SendType4"",""SendType5"",""SendType6"",""SendType7"",""SendType8"",""SendType9"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"",""No"",""Yes"",""int"",""float"",""undef"",""No"",""Yes"",""No"",""Yes"";
BA_DEF_  ""GenEnvVarEndingDsp"" STRING ;
BA_DEF_  ""GenEnvVarEndingSnd"" STRING ;
BA_DEF_  ""GenEnvVarPrefix"" STRING ;
BA_DEF_  ""Modified"" STRING ;
BA_DEF_ BO_  ""GenMsgBackgroundColor"" STRING ;
BA_DEF_ BO_  ""GenMsgForegroundColor"" STRING ;
BA_DEF_ BO_  ""matchingcriteria"" INT 0 0;
BA_DEF_ BO_  ""filterlabeling"" INT 0 0;
BA_DEF_ BO_  ""SystemMessageLongSymbol"" STRING ;
BA_DEF_DEF_  ""GenMsgSendType"" """";
BA_DEF_DEF_  ""GenMsgDelayTime"" 0;
BA_DEF_DEF_  ""GenMsgCycleTime"" 0;
BA_DEF_DEF_  ""GenMsgTimeoutTime"" 0;
BA_DEF_DEF_  ""ProtocolType"" """";
BA_DEF_DEF_  ""BusType"" """";
BA_DEF_DEF_  ""NodeLayerModules"" """";
BA_DEF_DEF_  ""ECU"" """";
BA_DEF_DEF_  ""CANoeJitterMax"" 0;
BA_DEF_DEF_  ""CANoeJitterMin"" 0;
BA_DEF_DEF_  ""CANoeDrift"" 0;
BA_DEF_DEF_  ""CANoeStartDelay"" 0;
BA_DEF_DEF_  ""GenMsgAutoGenSnd"" """";
BA_DEF_DEF_  ""GenMsgAutoGenDsp"" """";
BA_DEF_DEF_  ""GenSigAutoGenSnd"" """";
BA_DEF_DEF_  ""GenSigAutoGenDsp"" """";
BA_DEF_DEF_  ""GenSigEnvVarType"" """";
BA_DEF_DEF_  ""GenSigEVName"" """";
BA_DEF_DEF_  ""GenNodAutoGenSnd"" """";
BA_DEF_DEF_  ""GenNodAutoGenDsp"" """";
BA_DEF_DEF_  ""GenEnvVarEndingDsp"" """";
BA_DEF_DEF_  ""GenEnvVarEndingSnd"" """";
BA_DEF_DEF_  ""GenEnvVarPrefix"" """";
BA_DEF_DEF_  ""Modified"" ""JW2021Aug29"";
BA_DEF_DEF_  ""GenMsgBackgroundColor"" """";
BA_DEF_DEF_  ""GenMsgForegroundColor"" """";
BA_DEF_DEF_  ""matchingcriteria"" 0;
BA_DEF_DEF_  ""filterlabeling"" 0;
BA_DEF_DEF_  ""SystemMessageLongSymbol"" """";
BA_ ""BusType"" ""CAN"";
BA_ ""GenMsgCycleTime"" BO_ 530 100;
BA_ ""GenMsgSendType"" BO_ 530 0;
BA_ ""GenMsgCycleTime"" BO_ 819 500;
BA_ ""GenMsgCycleTime"" BO_ 577 100;
BA_ ""GenMsgCycleTime"" BO_ 468 10;
BA_ ""GenMsgSendType"" BO_ 468 0;
BA_ ""GenMsgCycleTime"" BO_ 962 50;
BA_ ""GenMsgCycleTime"" BO_ 822 1000;
BA_ ""GenMsgCycleTime"" BO_ 659 500;
BA_ ""GenMsgCycleTime"" BO_ 616 100;
BA_ ""GenMsgSendType"" BO_ 79 0;
BA_ ""GenMsgCycleTime"" BO_ 79 1000;
BA_ ""GenMsgCycleTime"" BO_ 978 1000;
BA_ ""GenMsgCycleTime"" BO_ 722 100;
BA_ ""GenMsgCycleTime"" BO_ 1345 100;
BA_ ""GenMsgCycleTime"" BO_ 580 100;
BA_ ""GenMsgCycleTime"" BO_ 532 100;
BA_ ""GenMsgCycleTime"" BO_ 533 1000;
BA_ ""GenMsgCycleTime"" BO_ 535 1000;
BA_ ""GenMsgCycleTime"" BO_ 801 1000;
BA_ ""SystemMessageLongSymbol"" BO_ 513 ""ID201VCFRONT_loggingAndVitals10Hz"";
BA_ ""GenMsgCycleTime"" BO_ 984 1000;
BA_ ""GenMsgCycleTime"" BO_ 609 100;
BA_ ""GenMsgSendType"" BO_ 297 0;
BA_ ""GenMsgCycleTime"" BO_ 297 10;
BA_ ""GenMsgCycleTime"" BO_ 612 100;
BA_ ""GenMsgCycleTime"" BO_ 548 100;
BA_ ""GenMsgSendType"" BO_ 280 0;
BA_ ""GenMsgCycleTime"" BO_ 280 10;
BA_ ""GenMsgCycleTime"" BO_ 850 1000;
BA_ ""GenMsgCycleTime"" BO_ 594 100;
BA_ ""GenMsgCycleTime"" BO_ 786 1000;
BA_ ""GenMsgCycleTime"" BO_ 658 100;
BA_ ""GenMsgCycleTime"" BO_ 599 20;
BA_ ""GenMsgCycleTime"" BO_ 1029 205;
BA_ ""GenMsgCycleTime"" BO_ 886 1000;
BA_ ""GenMsgCycleTime"" BO_ 340 10;
BA_ ""GenMsgSendType"" BO_ 340 0;
BA_ ""GenMsgCycleTime"" BO_ 950 1000;
BA_ ""GenMsgCycleTime"" BO_ 614 10;
BA_ ""GenMsgCycleTime"" BO_ 741 1000;
BA_ ""GenMsgSendType"" BO_ 264 0;
BA_ ""GenMsgCycleTime"" BO_ 264 10;
BA_ ""GenMsgCycleTime"" BO_ 306 10;
BA_ ""GenMsgSendType"" BO_ 306 0;
BA_ ""GenMsgSendType"" BO_ 294 0;
BA_ ""GenMsgCycleTime"" BO_ 294 100;
BA_ ""GenMsgCycleTime"" BO_ 799 1000;
BA_ ""GenMsgCycleTime"" BO_ 1022 1000;
BA_ ""GenMsgCycleTime"" BO_ 2047 100;
BA_ ""GenSigEnvVarType"" SG_ 472 TorqueFlags1D8 0;
VAL_ 12 UI_autopilotTrial 3 ""ACTIVE"" 0 ""NONE"" 1 ""START"" 2 ""STOP"" ;
VAL_ 12 UI_cellNetworkTechnology 9 ""CELL_NETWORK_CDMA"" 2 ""CELL_NETWORK_EDGE"" 1 ""CELL_NETWORK_GPRS"" 8 ""CELL_NETWORK_GSM"" 4 ""CELL_NETWORK_HSDPA"" 6 ""CELL_NETWORK_HSPA"" 5 ""CELL_NETWORK_HSUPA"" 7 ""CELL_NETWORK_LTE"" 0 ""CELL_NETWORK_NONE"" 15 ""CELL_NETWORK_SNA"" 3 ""CELL_NETWORK_UMTS"" 10 ""CELL_NETWORK_WCDMA"" ;
VAL_ 12 UI_cellSignalBars 5 ""FIVE"" 4 ""FOUR"" 1 ""ONE"" 7 ""SNA"" 3 ""THREE"" 2 ""TWO"" 0 ""ZERO"" ;
VAL_ 12 UI_factoryReset 3 ""CUSTOMER"" 1 ""DEVELOPER"" 2 ""DIAGNOSTIC"" 0 ""NONE_SNA"" ;
VAL_ 130 UI_energyAtDestination 32768 ""SNA"" 32767 ""TRIP_TOO_LONG"" ;
VAL_ 130 UI_hindsightEnergy 32768 ""SNA"" 32767 ""TRIP_TOO_LONG"" ;
VAL_ 130 UI_predictedEnergy 32768 ""SNA"" 32767 ""TRIP_TOO_LONG"" ;
VAL_ 257 RCM_pitchRate 16384 ""SNA"" ;
VAL_ 257 RCM_pitchRateQF 3 ""FAULTED"" 0 ""INIT"" 2 ""TEMP_INVALID"" 1 ""VALID"" ;
VAL_ 257 RCM_rollRate 16384 ""SNA"" ;
VAL_ 257 RCM_rollRateQF 3 ""FAULTED"" 0 ""INIT"" 2 ""TEMP_INVALID"" 1 ""VALID"" ;
VAL_ 257 RCM_yawRate 32768 ""SNA"" ;
VAL_ 257 RCM_yawRateQF 0 ""FAULTED"" 1 ""NOT_FAULTED"" ;
VAL_ 273 RCM_lateralAccel 32768 ""SNA"" ;
VAL_ 273 RCM_lateralAccelQF 0 ""FAULTED"" 1 ""NOT_FAULTED"" ;
VAL_ 273 RCM_longitudinalAccel 32768 ""SNA"" ;
VAL_ 273 RCM_longitudinalAccelQF 0 ""FAULTED"" 1 ""NOT_FAULTED"" ;
VAL_ 273 RCM_verticalAccel 32768 ""SNA"" ;
VAL_ 273 RCM_verticalAccelQF 0 ""FAULTED"" 1 ""NOT_FAULTED"" ;
VAL_ 258 VCLEFT_frontLatchStatus 5 ""LATCH_AJAR"" 2 ""LATCH_CLOSED"" 3 ""LATCH_CLOSING"" 7 ""LATCH_DEFAULT"" 8 ""LATCH_FAULT"" 1 ""LATCH_OPENED"" 4 ""LATCH_OPENING"" 0 ""LATCH_SNA"" 6 ""LATCH_TIMEOUT"" ;
VAL_ 258 VCLEFT_mirrorFoldState 1 ""MIRROR_FOLD_STATE_FOLDED"" 3 ""MIRROR_FOLD_STATE_FOLDING"" 2 ""MIRROR_FOLD_STATE_UNFOLDED"" 4 ""MIRROR_FOLD_STATE_UNFOLDING"" 0 ""MIRROR_FOLD_STATE_UNKNOWN"" ;
VAL_ 258 VCLEFT_mirrorHeatState 4 ""HEATER_STATE_FAULT"" 2 ""HEATER_STATE_OFF"" 3 ""HEATER_STATE_OFF_UNAVAILABLE"" 1 ""HEATER_STATE_ON"" 0 ""HEATER_STATE_SNA"" ;
VAL_ 258 VCLEFT_mirrorRecallState 0 ""MIRROR_RECALL_STATE_INIT"" 1 ""MIRROR_RECALL_STATE_RECALLING_AXIS_1"" 2 ""MIRROR_RECALL_STATE_RECALLING_AXIS_2"" 3 ""MIRROR_RECALL_STATE_RECALLING_COMPLETE"" 4 ""MIRROR_RECALL_STATE_RECALLING_FAILED"" 5 ""MIRROR_RECALL_STATE_RECALLING_STOPPED"" ;
VAL_ 258 VCLEFT_mirrorState 3 ""MIRROR_STATE_FOLD_UNFOLD"" 0 ""MIRROR_STATE_IDLE"" 4 ""MIRROR_STATE_RECALL"" 1 ""MIRROR_STATE_TILT_X"" 2 ""MIRROR_STATE_TILT_Y"" ;
VAL_ 258 VCLEFT_rearLatchStatus 5 ""LATCH_AJAR"" 2 ""LATCH_CLOSED"" 3 ""LATCH_CLOSING"" 7 ""LATCH_DEFAULT"" 8 ""LATCH_FAULT"" 1 ""LATCH_OPENED"" 4 ""LATCH_OPENING"" 0 ""LATCH_SNA"" 6 ""LATCH_TIMEOUT"" ;
VAL_ 259 VCRIGHT_frontLatchStatus 5 ""LATCH_AJAR"" 2 ""LATCH_CLOSED"" 3 ""LATCH_CLOSING"" 7 ""LATCH_DEFAULT"" 8 ""LATCH_FAULT"" 1 ""LATCH_OPENED"" 4 ""LATCH_OPENING"" 0 ""LATCH_SNA"" 6 ""LATCH_TIMEOUT"" ;
VAL_ 259 VCRIGHT_mirrorFoldState 1 ""MIRROR_FOLD_STATE_FOLDED"" 3 ""MIRROR_FOLD_STATE_FOLDING"" 2 ""MIRROR_FOLD_STATE_UNFOLDED"" 4 ""MIRROR_FOLD_STATE_UNFOLDING"" 0 ""MIRROR_FOLD_STATE_UNKNOWN"" ;
VAL_ 259 VCRIGHT_mirrorRecallState 0 ""MIRROR_RECALL_STATE_INIT"" 1 ""MIRROR_RECALL_STATE_RECALLING_AXIS_1"" 2 ""MIRROR_RECALL_STATE_RECALLING_AXIS_2"" 3 ""MIRROR_RECALL_STATE_RECALLING_COMPLETE"" 4 ""MIRROR_RECALL_STATE_RECALLING_FAILED"" 5 ""MIRROR_RECALL_STATE_RECALLING_STOPPED"" ;
VAL_ 259 VCRIGHT_mirrorState 3 ""MIRROR_STATE_FOLD_UNFOLD"" 0 ""MIRROR_STATE_IDLE"" 4 ""MIRROR_STATE_RECALL"" 1 ""MIRROR_STATE_TILT_X"" 2 ""MIRROR_STATE_TILT_Y"" ;
VAL_ 259 VCRIGHT_rearLatchStatus 5 ""LATCH_AJAR"" 2 ""LATCH_CLOSED"" 3 ""LATCH_CLOSING"" 7 ""LATCH_DEFAULT"" 8 ""LATCH_FAULT"" 1 ""LATCH_OPENED"" 4 ""LATCH_OPENING"" 0 ""LATCH_SNA"" 6 ""LATCH_TIMEOUT"" ;
VAL_ 259 VCRIGHT_trunkLatchStatus 5 ""LATCH_AJAR"" 2 ""LATCH_CLOSED"" 3 ""LATCH_CLOSING"" 7 ""LATCH_DEFAULT"" 8 ""LATCH_FAULT"" 1 ""LATCH_OPENED"" 4 ""LATCH_OPENING"" 0 ""LATCH_SNA"" 6 ""LATCH_TIMEOUT"" ;
VAL_ 275 GTW_bmpState 2 ""BMP_STATE_ASLEEP"" 3 ""BMP_STATE_MIA"" 0 ""BMP_STATE_OFF"" 1 ""BMP_STATE_ON"" 5 ""BMP_STATE_POWER_CYCLE"" 4 ""BMP_STATE_RESET"" 255 ""DUMMY"" ;
VAL_ 281 VCSEC_windowRequestPercent 127 ""SNA"" ;
VAL_ 281 VCSEC_windowRequestType 3 ""WINDOW_REQUEST_GOTO_CLOSED"" 2 ""WINDOW_REQUEST_GOTO_CRACKED"" 1 ""WINDOW_REQUEST_GOTO_PERCENT"" 0 ""WINDOW_REQUEST_IDLE"" ;
VAL_ 290 VCLEFT_frontDoorState 1 ""DOOR_STATE_CLOSED"" 4 ""DOOR_STATE_OPEN_OR_AJAR"" 3 ""DOOR_STATE_RELEASING_LATCH"" 0 ""DOOR_STATE_UNKNOWN"" 2 ""DOOR_STATE_WAIT_FOR_SHORT_DROP"" ;
VAL_ 290 VCLEFT_frontHandleDebounceStatus 3 ""EXTERIOR_HANDLE_STATUS_ACTIVE"" 4 ""EXTERIOR_HANDLE_STATUS_DISCONNECTED"" 5 ""EXTERIOR_HANDLE_STATUS_FAULT"" 1 ""EXTERIOR_HANDLE_STATUS_INDETERMINATE"" 2 ""EXTERIOR_HANDLE_STATUS_NOT_ACTIVE"" 0 ""EXTERIOR_HANDLE_STATUS_SNA"" ;
VAL_ 290 VCLEFT_frontHandleRawStatus 3 ""EXTERIOR_HANDLE_STATUS_ACTIVE"" 4 ""EXTERIOR_HANDLE_STATUS_DISCONNECTED"" 5 ""EXTERIOR_HANDLE_STATUS_FAULT"" 1 ""EXTERIOR_HANDLE_STATUS_INDETERMINATE"" 2 ""EXTERIOR_HANDLE_STATUS_NOT_ACTIVE"" 0 ""EXTERIOR_HANDLE_STATUS_SNA"" ;
VAL_ 290 VCLEFT_rearDoorState 1 ""DOOR_STATE_CLOSED"" 4 ""DOOR_STATE_OPEN_OR_AJAR"" 3 ""DOOR_STATE_RELEASING_LATCH"" 0 ""DOOR_STATE_UNKNOWN"" 2 ""DOOR_STATE_WAIT_FOR_SHORT_DROP"" ;
VAL_ 290 VCLEFT_rearHandleDebounceStatus 3 ""EXTERIOR_HANDLE_STATUS_ACTIVE"" 4 ""EXTERIOR_HANDLE_STATUS_DISCONNECTED"" 5 ""EXTERIOR_HANDLE_STATUS_FAULT"" 1 ""EXTERIOR_HANDLE_STATUS_INDETERMINATE"" 2 ""EXTERIOR_HANDLE_STATUS_NOT_ACTIVE"" 0 ""EXTERIOR_HANDLE_STATUS_SNA"" ;
VAL_ 290 VCLEFT_rearHandleRawStatus 3 ""EXTERIOR_HANDLE_STATUS_ACTIVE"" 4 ""EXTERIOR_HANDLE_STATUS_DISCONNECTED"" 5 ""EXTERIOR_HANDLE_STATUS_FAULT"" 1 ""EXTERIOR_HANDLE_STATUS_INDETERMINATE"" 2 ""EXTERIOR_HANDLE_STATUS_NOT_ACTIVE"" 0 ""EXTERIOR_HANDLE_STATUS_SNA"" ;
VAL_ 322 VCLEFT_liftgateStatusIndex 0 ""LIFTGATE_STATUS_INDEX_0"" 1 ""LIFTGATE_STATUS_INDEX_1"" 2 ""LIFTGATE_STATUS_INDEX_INVALID"" ;
VAL_ 322 VCLEFT_liftgateLatchRequest 1 ""LATCH_REQUEST_CINCH"" 3 ""LATCH_REQUEST_FORCE_RELEASE"" 0 ""LATCH_REQUEST_NONE"" 2 ""LATCH_REQUEST_RELEASE"" 4 ""LATCH_REQUEST_RESET"" ;
VAL_ 322 VCLEFT_liftgateMvmntNotAllowedCo 4 ""PLG_MVMT_NOT_ALLOWED_EXTERIOR_PRESS_AT_MAX_OPEN"" 5 ""PLG_MVMT_NOT_ALLOWED_LOCKED"" 1 ""PLG_MVMT_NOT_ALLOWED_LOW_12V"" 0 ""PLG_MVMT_NOT_ALLOWED_NONE"" 3 ""PLG_MVMT_NOT_ALLOWED_UNCALIBRATED"" 2 ""PLG_MVMT_NOT_ALLOWED_VEHICLE_AT_SPEED"" ;
VAL_ 322 VCLEFT_liftgatePhysicalChimeRequ 0 ""LIFTGATE_CHIME_REQUEST_NONE"" 4 ""LIFTGATE_CHIME_REQUEST_ONE_LONG"" 1 ""LIFTGATE_CHIME_REQUEST_ONE_SHORT"" 3 ""LIFTGATE_CHIME_REQUEST_THREE_SHORT"" 2 ""LIFTGATE_CHIME_REQUEST_TWO_SHORT"" ;
VAL_ 322 VCLEFT_liftgateRequestSource 5 ""PLG_REQUEST_SOURCE_CLOSE_ALL"" 2 ""PLG_REQUEST_SOURCE_EXTERIOR"" 4 ""PLG_REQUEST_SOURCE_KEY_TRUNK_BUTTON"" 6 ""PLG_REQUEST_SOURCE_MCU_CLOSE"" 1 ""PLG_REQUEST_SOURCE_MCU_SWITCH"" 0 ""PLG_REQUEST_SOURCE_NONE"" 3 ""PLG_REQUEST_SOURCE_SHUTFACE"" 7 ""PLG_REQUEST_SOURCE_UDS"" ;
VAL_ 322 VCLEFT_liftgateState 2 ""PLG_STATE_BACKOFF"" 5 ""PLG_STATE_CLOSED"" 4 ""PLG_STATE_CLOSING"" 11 ""PLG_STATE_END_OF_TRAVEL"" 0 ""PLG_STATE_INIT"" 7 ""PLG_STATE_LATCH_CLOSING"" 12 ""PLG_STATE_LATCH_ENTRY"" 10 ""PLG_STATE_LATCH_EXIT"" 6 ""PLG_STATE_LATCH_OPENING"" 8 ""PLG_STATE_NOT_INSTALLED"" 1 ""PLG_STATE_OFF"" 3 ""PLG_STATE_OPENING"" 9 ""PLG_STATE_UNKNOWN"" ;
VAL_ 322 VCLEFT_liftgateStoppingCondition 11 ""PLG_STOPPING_CONDITION_COUNT"" 10 ""PLG_STOPPING_CONDITION_LATCH_FAULT"" 3 ""PLG_STOPPING_CONDITION_LOW_12V"" 0 ""PLG_STOPPING_CONDITION_NONE"" 6 ""PLG_STOPPING_CONDITION_OBSTACLE_CURRENT"" 2 ""PLG_STOPPING_CONDITION_OBSTACLE_STALL"" 7 ""PLG_STOPPING_CONDITION_OBSTACLE_TRAJ_POS"" 8 ""PLG_STOPPING_CONDITION_OBSTACLE_TRAJ_VEL"" 1 ""PLG_STOPPING_CONDITION_PINCH"" 4 ""PLG_STOPPING_CONDITION_STATE_TIMEOUT"" 9 ""PLG_STOPPING_CONDITION_UNCALIBRATED"" 5 ""PLG_STOPPING_CONDITION_VEHICLE_AT_SPEED"" ;
VAL_ 322 VCLEFT_liftgateUIChimeRequest 0 ""LIFTGATE_CHIME_REQUEST_NONE"" 4 ""LIFTGATE_CHIME_REQUEST_ONE_LONG"" 1 ""LIFTGATE_CHIME_REQUEST_ONE_SHORT"" 3 ""LIFTGATE_CHIME_REQUEST_THREE_SHORT"" 2 ""LIFTGATE_CHIME_REQUEST_TWO_SHORT"" ;
VAL_ 325 ESP_absBrakeEvent2 2 ""ABS_EVENT_ACTIVE_FRONT"" 1 ""ABS_EVENT_ACTIVE_FRONT_REAR"" 3 ""ABS_EVENT_ACTIVE_REAR"" 0 ""ABS_EVENT_NOT_ACTIVE"" ;
VAL_ 325 ESP_absFaultLamp 0 ""ABS_FAULT_LAMP_OFF"" 1 ""ABS_FAULT_LAMP_ON"" ;
VAL_ 325 ESP_brakeApply 1 ""BLS_ACTIVE"" 0 ""BLS_INACTIVE"" ;
VAL_ 325 ESP_brakeDiscWipingActive 1 ""BDW_ACTIVE"" 0 ""BDW_INACTIVE"" ;
VAL_ 325 ESP_brakeLamp 0 ""LAMP_OFF"" 1 ""LAMP_ON"" ;
VAL_ 325 ESP_brakeTorqueTarget 8191 ""SNA"" ;
VAL_ 325 ESP_btcTargetState 1 ""BACKUP"" 0 ""OFF"" 2 ""ON"" 3 ""SNA"" ;
VAL_ 325 ESP_cdpStatus 2 ""ACTUATING_EPB_CDP"" 3 ""CDP_COMMAND_INVALID"" 1 ""CDP_IS_AVAILABLE"" 0 ""CDP_IS_NOT_AVAILABLE"" ;
VAL_ 325 ESP_driverBrakeApply 2 ""Driver_applying_brakes"" 3 ""Faulty_SNA"" 0 ""NotInit_orOff"" 1 ""Not_Applied"" ;
VAL_ 325 ESP_ebdFaultLamp 0 ""EBD_FAULT_LAMP_OFF"" 1 ""EBD_FAULT_LAMP_ON"" ;
VAL_ 325 ESP_ebrStandstillSkid 0 ""NO_STANDSTILL_SKID"" 1 ""STANDSTILL_SKID_DETECTED"" ;
VAL_ 325 ESP_ebrStatus 2 ""ACTUATING_DI_EBR"" 3 ""EBR_COMMAND_INVALID"" 1 ""EBR_IS_AVAILABLE"" 0 ""EBR_IS_NOT_AVAILABLE"" ;
VAL_ 325 ESP_espFaultLamp 0 ""ESP_FAULT_LAMP_OFF"" 1 ""ESP_FAULT_LAMP_ON"" ;
VAL_ 325 ESP_espLampFlash 1 ""ESP_LAMP_FLASH"" 0 ""ESP_LAMP_OFF"" ;
VAL_ 325 ESP_espModeActive 0 ""ESP_MODE_00_NORMAL"" 1 ""ESP_MODE_01"" 2 ""ESP_MODE_02"" 3 ""ESP_MODE_03"" ;
VAL_ 325 ESP_lateralAccelQF 1 ""IN_SPEC"" 0 ""UNDEFINABLE_ACCURACY"" ;
VAL_ 325 ESP_longitudinalAccelQF 1 ""IN_SPEC"" 0 ""UNDEFINABLE_ACCURACY"" ;
VAL_ 325 ESP_ptcTargetState 1 ""BACKUP"" 0 ""FAULT"" 2 ""ON"" 3 ""SNA"" ;
VAL_ 325 ESP_stabilityControlSts2 2 ""ENGAGED"" 3 ""FAULTED"" 0 ""INIT"" 1 ""ON"" ;
VAL_ 325 ESP_steeringAngleQF 1 ""IN_SPEC"" 0 ""UNDEFINABLE_ACCURACY"" ;
VAL_ 325 ESP_yawRateQF 1 ""IN_SPEC"" 0 ""UNDEFINABLE_ACCURACY"" ;
VAL_ 522 HVP_fcContNegativeState 3 ""CONTACTOR_STATE_BLOCKED"" 6 ""CONTACTOR_STATE_ECONOMIZED"" 1 ""CONTACTOR_STATE_OPEN"" 5 ""CONTACTOR_STATE_OPENING"" 2 ""CONTACTOR_STATE_PRECHARGE"" 4 ""CONTACTOR_STATE_PULLED_IN"" 0 ""CONTACTOR_STATE_SNA"" 7 ""CONTACTOR_STATE_WELDED"" ;
VAL_ 522 HVP_fcContPositiveState 3 ""CONTACTOR_STATE_BLOCKED"" 6 ""CONTACTOR_STATE_ECONOMIZED"" 1 ""CONTACTOR_STATE_OPEN"" 5 ""CONTACTOR_STATE_OPENING"" 2 ""CONTACTOR_STATE_PRECHARGE"" 4 ""CONTACTOR_STATE_PULLED_IN"" 0 ""CONTACTOR_STATE_SNA"" 7 ""CONTACTOR_STATE_WELDED"" ;
VAL_ 522 HVP_fcContactorSetState 3 ""CONTACTOR_SET_STATE_BLOCKED"" 5 ""CONTACTOR_SET_STATE_CLOSED"" 2 ""CONTACTOR_SET_STATE_CLOSING"" 9 ""CONTACTOR_SET_STATE_NEGATIVE_CLOSED"" 1 ""CONTACTOR_SET_STATE_OPEN"" 4 ""CONTACTOR_SET_STATE_OPENING"" 6 ""CONTACTOR_SET_STATE_PARTIAL_WELD"" 8 ""CONTACTOR_SET_STATE_POSITIVE_CLOSED"" 0 ""CONTACTOR_SET_STATE_SNA"" 7 ""CONTACTOR_SET_STATE_WELDED"" ;
VAL_ 522 HVP_fcCtrsRequestStatus 1 ""REQUEST_ACTIVE"" 2 ""REQUEST_COMPLETED"" 0 ""REQUEST_NOT_ACTIVE"" ;
VAL_ 522 HVP_fcLinkAllowedToEnergize 1 ""FC_LINK_ENERGY_AC"" 2 ""FC_LINK_ENERGY_DC"" 0 ""FC_LINK_ENERGY_NONE"" ;
VAL_ 522 HVP_hvilStatus 2 ""CURRENT_SOURCE_FAULT"" 3 ""INTERNAL_OPEN_FAULT"" 8 ""NO_12V_SUPPLY"" 5 ""PENTHOUSE_LID_OPEN_FAULT"" 1 ""STATUS_OK"" 0 ""UNKNOWN"" 6 ""UNKNOWN_LOCATION_OPEN_FAULT"" 7 ""VEHICLE_NODE_FAULT"" 4 ""VEHICLE_OPEN_FAULT"" 9 ""VEHICLE_OR_PENTHOUSE_LID_OPEN_FAULT"" ;
VAL_ 522 HVP_packContNegativeState 3 ""CONTACTOR_STATE_BLOCKED"" 6 ""CONTACTOR_STATE_ECONOMIZED"" 1 ""CONTACTOR_STATE_OPEN"" 5 ""CONTACTOR_STATE_OPENING"" 2 ""CONTACTOR_STATE_PRECHARGE"" 4 ""CONTACTOR_STATE_PULLED_IN"" 0 ""CONTACTOR_STATE_SNA"" 7 ""CONTACTOR_STATE_WELDED"" ;
VAL_ 522 HVP_packContPositiveState 3 ""CONTACTOR_STATE_BLOCKED"" 6 ""CONTACTOR_STATE_ECONOMIZED"" 1 ""CONTACTOR_STATE_OPEN"" 5 ""CONTACTOR_STATE_OPENING"" 2 ""CONTACTOR_STATE_PRECHARGE"" 4 ""CONTACTOR_STATE_PULLED_IN"" 0 ""CONTACTOR_STATE_SNA"" 7 ""CONTACTOR_STATE_WELDED"" ;
VAL_ 522 HVP_packContactorSetState 3 ""CONTACTOR_SET_STATE_BLOCKED"" 5 ""CONTACTOR_SET_STATE_CLOSED"" 2 ""CONTACTOR_SET_STATE_CLOSING"" 9 ""CONTACTOR_SET_STATE_NEGATIVE_CLOSED"" 1 ""CONTACTOR_SET_STATE_OPEN"" 4 ""CONTACTOR_SET_STATE_OPENING"" 6 ""CONTACTOR_SET_STATE_PARTIAL_WELD"" 8 ""CONTACTOR_SET_STATE_POSITIVE_CLOSED"" 0 ""CONTACTOR_SET_STATE_SNA"" 7 ""CONTACTOR_SET_STATE_WELDED"" ;
VAL_ 522 HVP_packCtrsRequestStatus 1 ""REQUEST_ACTIVE"" 2 ""REQUEST_COMPLETED"" 0 ""REQUEST_NOT_ACTIVE"" ;
VAL_ 526 PARK_sdiSensor1RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 526 PARK_sdiSensor2RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 526 PARK_sdiSensor3RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 526 PARK_sdiSensor4RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 526 PARK_sdiSensor5RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 526 PARK_sdiSensor6RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 537 VCSEC_TPMSDataIndex 0 ""TPMS_DATA_SENSOR_0"" 1 ""TPMS_DATA_SENSOR_1"" 2 ""TPMS_DATA_SENSOR_2"" 3 ""TPMS_DATA_SENSOR_3"" ;
VAL_ 537 VCSEC_TPMSBatVoltage0 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSBatVoltage1 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSBatVoltage2 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSBatVoltage3 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSLocation0 0 ""LOCATION_FL"" 1 ""LOCATION_FR"" 2 ""LOCATION_RL"" 3 ""LOCATION_RR"" 4 ""LOCATION_UNKNOWN"" ;
VAL_ 537 VCSEC_TPMSLocation1 0 ""LOCATION_FL"" 1 ""LOCATION_FR"" 2 ""LOCATION_RL"" 3 ""LOCATION_RR"" 4 ""LOCATION_UNKNOWN"" ;
VAL_ 537 VCSEC_TPMSLocation2 0 ""LOCATION_FL"" 1 ""LOCATION_FR"" 2 ""LOCATION_RL"" 3 ""LOCATION_RR"" 4 ""LOCATION_UNKNOWN"" ;
VAL_ 537 VCSEC_TPMSLocation3 0 ""LOCATION_FL"" 1 ""LOCATION_FR"" 2 ""LOCATION_RL"" 3 ""LOCATION_RR"" 4 ""LOCATION_UNKNOWN"" ;
VAL_ 537 VCSEC_TPMSPressure0 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSPressure1 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSPressure2 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSPressure3 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSTemperature0 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSTemperature1 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSTemperature2 255 ""SNA"" ;
VAL_ 537 VCSEC_TPMSTemperature3 255 ""SNA"" ;
VAL_ 516 PCS_chargeShutdownRequest 2 ""EMERGENCY_SHUTDOWN_REQUESTED"" 1 ""GRACEFUL_SHUTDOWN_REQUESTED"" 0 ""NO_SHUTDOWN_REQUESTED"" ;
VAL_ 516 PCS_chgMainState 9 ""PCS_CHG_STATE_CLEAR_FAULTS"" 6 ""PCS_CHG_STATE_ENABLE"" 8 ""PCS_CHG_STATE_FAULTED"" 1 ""PCS_CHG_STATE_IDLE"" 0 ""PCS_CHG_STATE_INIT"" 4 ""PCS_CHG_STATE_QUALIFY_LINE_CONFIG"" 7 ""PCS_CHG_STATE_SHUTDOWN"" 2 ""PCS_CHG_STATE_STARTUP"" 5 ""PCS_CHG_STATE_SYSTEM_CONFIG"" 3 ""PCS_CHG_STATE_WAIT_FOR_LINE_VOLTAGE"" ;
VAL_ 516 PCS_gridConfig 1 ""GRID_CONFIG_SINGLE_PHASE"" 0 ""GRID_CONFIG_SNA"" 2 ""GRID_CONFIG_THREE_PHASE"" 3 ""GRID_CONFIG_THREE_PHASE_DELTA"" ;
VAL_ 516 PCS_hvChargeStatus 1 ""PCS_CHARGE_BLOCKED"" 2 ""PCS_CHARGE_ENABLED"" 3 ""PCS_CHARGE_FAULTED"" 0 ""PCS_CHARGE_STANDBY"" ;
VAL_ 516 PCS_hwVariantType 1 ""PCS_32A_SINGLE_PHASE_VARIANT"" 0 ""PCS_48A_SINGLE_PHASE_VARIANT"" 3 ""PCS_HW_VARIANT_TYPE_SNA"" 2 ""PCS_THREE_PHASES_VARIANT"" ;
VAL_ 554 HVP_dcLinkVoltageFiltered 550 ""SNA"" ;
VAL_ 554 HVP_pcsControlRequest 3 ""DISCHARGE"" 2 ""PRECHARGE"" 0 ""SHUTDOWN"" 1 ""SUPPORT"" ;
VAL_ 562 BMS_fcContactorRequest 1 ""SET_REQUEST_CLOSE"" 4 ""SET_REQUEST_CLOSE_NEGATIVE_ONLY"" 5 ""SET_REQUEST_CLOSE_POSITIVE_ONLY"" 2 ""SET_REQUEST_OPEN"" 3 ""SET_REQUEST_OPEN_IMMEDIATELY"" 0 ""SET_REQUEST_SNA"" ;
VAL_ 562 BMS_fcLinkOkToEnergizeRequest 1 ""FC_LINK_ENERGY_AC"" 2 ""FC_LINK_ENERGY_DC"" 0 ""FC_LINK_ENERGY_NONE"" ;
VAL_ 562 BMS_internalHvilSenseV 65535 ""SNA"" ;
VAL_ 562 BMS_packContactorRequest 1 ""SET_REQUEST_CLOSE"" 4 ""SET_REQUEST_CLOSE_NEGATIVE_ONLY"" 5 ""SET_REQUEST_CLOSE_POSITIVE_ONLY"" 2 ""SET_REQUEST_OPEN"" 3 ""SET_REQUEST_OPEN_IMMEDIATELY"" 0 ""SET_REQUEST_SNA"" ;
VAL_ 609 VCFRONT_batterySMState 0 ""INIT"" 1 ""CHARGE"" 2 ""DISCHARGE"" 3 ""STANDBY"" 4 ""RESISTANCE_ESTIMATION"" 5 ""OTA_STANDBY"" 6 ""DISCONNECTED_BATTERY_TEST"" 7 ""SHORTED_CELL_TEST"" 8 ""FAULT"" 9 ""RECOVERY"" ;
VAL_ 609 VCFRONT_voltageProfile 0 ""CHARGE"" 1 ""FLOAT"" 2 ""REDUCED_FLOAT"" 3 ""ALWAYS_CLOSED_CONTACTORS"" ;
VAL_ 627 UI_displayBrightnessLevel 255 ""SNA"" ;
VAL_ 627 UI_domeLightSwitch 2 ""DOME_LIGHT_SWITCH_AUTO"" 0 ""DOME_LIGHT_SWITCH_OFF"" 1 ""DOME_LIGHT_SWITCH_ON"" ;
VAL_ 627 UI_driveStateRequest 0 ""DRIVE_STATE_REQ_IDLE"" 1 ""DRIVE_STATE_REQ_START"" ;
VAL_ 627 UI_frontLeftSeatHeatReq 1 ""HEATER_REQUEST_LEVEL1"" 2 ""HEATER_REQUEST_LEVEL2"" 3 ""HEATER_REQUEST_LEVEL3"" 0 ""HEATER_REQUEST_OFF"" ;
VAL_ 627 UI_frontRightSeatHeatReq 1 ""HEATER_REQUEST_LEVEL1"" 2 ""HEATER_REQUEST_LEVEL2"" 3 ""HEATER_REQUEST_LEVEL3"" 0 ""HEATER_REQUEST_OFF"" ;
VAL_ 627 UI_lockRequest 0 ""UI_LOCK_REQUEST_IDLE"" 1 ""UI_LOCK_REQUEST_LOCK"" 4 ""UI_LOCK_REQUEST_REMOTE_LOCK"" 3 ""UI_LOCK_REQUEST_REMOTE_UNLOCK"" 7 ""UI_LOCK_REQUEST_SNA"" 2 ""UI_LOCK_REQUEST_UNLOCK"" ;
VAL_ 627 UI_mirrorFoldRequest 0 ""MIRROR_FOLD_REQUEST_IDLE"" 2 ""MIRROR_FOLD_REQUEST_PRESENT"" 1 ""MIRROR_FOLD_REQUEST_RETRACT"" 3 ""MIRROR_FOLD_REQUEST_SNA"" ;
VAL_ 627 UI_rearCenterSeatHeatReq 1 ""HEATER_REQUEST_LEVEL1"" 2 ""HEATER_REQUEST_LEVEL2"" 3 ""HEATER_REQUEST_LEVEL3"" 0 ""HEATER_REQUEST_OFF"" ;
VAL_ 627 UI_rearLeftSeatHeatReq 1 ""HEATER_REQUEST_LEVEL1"" 2 ""HEATER_REQUEST_LEVEL2"" 3 ""HEATER_REQUEST_LEVEL3"" 0 ""HEATER_REQUEST_OFF"" ;
VAL_ 627 UI_rearRightSeatHeatReq 1 ""HEATER_REQUEST_LEVEL1"" 2 ""HEATER_REQUEST_LEVEL2"" 3 ""HEATER_REQUEST_LEVEL3"" 0 ""HEATER_REQUEST_OFF"" ;
VAL_ 627 UI_remoteClosureRequest 2 ""UI_REMOTE_CLOSURE_REQUEST_FRONT_TRUNK_MOVE"" 0 ""UI_REMOTE_CLOSURE_REQUEST_IDLE"" 1 ""UI_REMOTE_CLOSURE_REQUEST_REAR_TRUNK_MOVE"" 3 ""UI_REMOTE_CLOSURE_REQUEST_SNA"" ;
VAL_ 627 UI_remoteStartRequest 0 ""UI_REMOTE_START_REQUEST_IDLE"" 4 ""UI_REMOTE_START_REQUEST_SNA"" 1 ""UI_REMOTE_START_REQUEST_START"" ;
VAL_ 627 UI_steeringBacklightEnabled 0 ""STEERING_BACKLIGHT_DISABLED"" 1 ""STEERING_BACKLIGHT_ENABLED"" ;
VAL_ 627 UI_steeringButtonMode 4 ""STEERING_BUTTON_MODE_HEADLIGHT_LEFT"" 5 ""STEERING_BUTTON_MODE_HEADLIGHT_RIGHT"" 2 ""STEERING_BUTTON_MODE_MIRROR_LEFT"" 3 ""STEERING_BUTTON_MODE_MIRROR_RIGHT"" 0 ""STEERING_BUTTON_MODE_OFF"" 1 ""STEERING_BUTTON_MODE_STEERING_COLUMN_ADJ"" ;
VAL_ 627 UI_wiperMode 2 ""WIPER_MODE_NORMAL"" 3 ""WIPER_MODE_PARK"" 1 ""WIPER_MODE_SERVICE"" 0 ""WIPER_MODE_SNA"" ;
VAL_ 627 UI_wiperRequest 2 ""WIPER_REQUEST_AUTO"" 6 ""WIPER_REQUEST_FAST_CONTINUOUS"" 4 ""WIPER_REQUEST_FAST_INTERMITTENT"" 1 ""WIPER_REQUEST_OFF"" 5 ""WIPER_REQUEST_SLOW_CONTINUOUS"" 3 ""WIPER_REQUEST_SLOW_INTERMITTENT"" 0 ""WIPER_REQUEST_SNA"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeCurrent0 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeCurrent1 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeCurrent2 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeCurrent3 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeDesired0 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeDesired1 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeDesired2 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSConnectionTypeDesired3 0 ""CONNECTIONTYPE_FAST"" 1 ""CONNECTIONTYPE_SLOW"" 2 ""CONNECTIONTYPE_UNKNOWN"" ;
VAL_ 1066 VCSEC_TPMSSensorState0 3 ""SENSOR_CONNECTED"" 4 ""SENSOR_DISCONNECTING"" 0 ""SENSOR_NOT_PAIRED"" 1 ""SENSOR_WAIT_FOR_ADV"" 2 ""SENSOR_WAIT_FOR_CONN"" ;
VAL_ 1066 VCSEC_TPMSSensorState1 3 ""SENSOR_CONNECTED"" 4 ""SENSOR_DISCONNECTING"" 0 ""SENSOR_NOT_PAIRED"" 1 ""SENSOR_WAIT_FOR_ADV"" 2 ""SENSOR_WAIT_FOR_CONN"" ;
VAL_ 1066 VCSEC_TPMSSensorState2 3 ""SENSOR_CONNECTED"" 4 ""SENSOR_DISCONNECTING"" 0 ""SENSOR_NOT_PAIRED"" 1 ""SENSOR_WAIT_FOR_ADV"" 2 ""SENSOR_WAIT_FOR_CONN"" ;
VAL_ 1066 VCSEC_TPMSSensorState3 3 ""SENSOR_CONNECTED"" 4 ""SENSOR_DISCONNECTING"" 0 ""SENSOR_NOT_PAIRED"" 1 ""SENSOR_WAIT_FOR_ADV"" 2 ""SENSOR_WAIT_FOR_CONN"" ;
VAL_ 558 PARK_sdiSensor10RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 558 PARK_sdiSensor11RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 558 PARK_sdiSensor12RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 558 PARK_sdiSensor7RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 558 PARK_sdiSensor8RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 558 PARK_sdiSensor9RawDistData 0 ""BLOCKED"" 1 ""NEAR_DETECTION"" 500 ""NO_OBJECT_DETECTED"" 511 ""SNA"" ;
VAL_ 568 UI_countryCode 1023 ""SNA"" 0 ""UNKNOWN"" ;
VAL_ 568 UI_mapSpeedLimit 3 ""LESS_OR_EQ_10"" 21 ""LESS_OR_EQ_100"" 22 ""LESS_OR_EQ_105"" 23 ""LESS_OR_EQ_110"" 24 ""LESS_OR_EQ_115"" 25 ""LESS_OR_EQ_120"" 26 ""LESS_OR_EQ_130"" 27 ""LESS_OR_EQ_140"" 4 ""LESS_OR_EQ_15"" 28 ""LESS_OR_EQ_150"" 29 ""LESS_OR_EQ_160"" 5 ""LESS_OR_EQ_20"" 6 ""LESS_OR_EQ_25"" 7 ""LESS_OR_EQ_30"" 8 ""LESS_OR_EQ_35"" 9 ""LESS_OR_EQ_40"" 10 ""LESS_OR_EQ_45"" 1 ""LESS_OR_EQ_5"" 11 ""LESS_OR_EQ_50"" 12 ""LESS_OR_EQ_55"" 13 ""LESS_OR_EQ_60"" 14 ""LESS_OR_EQ_65"" 2 ""LESS_OR_EQ_7"" 15 ""LESS_OR_EQ_70"" 16 ""LESS_OR_EQ_75"" 17 ""LESS_OR_EQ_80"" 18 ""LESS_OR_EQ_85"" 19 ""LESS_OR_EQ_90"" 20 ""LESS_OR_EQ_95"" 31 ""SNA"" 0 ""UNKNOWN"" 30 ""UNLIMITED"" ;
VAL_ 568 UI_mapSpeedLimitDependency 6 ""LANE"" 0 ""NONE"" 2 ""RAIN"" 1 ""SCHOOL"" 5 ""SEASON"" 7 ""SNA"" 3 ""SNOW"" 4 ""TIME"" ;
VAL_ 568 UI_mapSpeedLimitType 2 ""ADVISORY"" 4 ""BUMPS"" 3 ""DEPENDENT"" 1 ""REGULAR"" 7 ""UNKNOWN_SNA"" ;
VAL_ 568 UI_mapSpeedUnits 1 ""KPH"" 0 ""MPH"" ;
VAL_ 568 UI_nextBranchDist 31 ""SNA"" ;
VAL_ 568 UI_roadClass 1 ""CLASS_1_MAJOR"" 2 ""CLASS_2"" 3 ""CLASS_3"" 4 ""CLASS_4"" 5 ""CLASS_5"" 6 ""CLASS_6_MINOR"" 0 ""UNKNOWN_INVALID_SNA"" ;
VAL_ 569 DAS_leftFork 1 ""LEFT_FORK_AVAILABLE"" 0 ""LEFT_FORK_NONE"" 2 ""LEFT_FORK_SELECTED"" 3 ""LEFT_FORK_UNAVAILABLE"" ;
VAL_ 569 DAS_leftLineUsage 1 ""AVAILABLE"" 3 ""BLACKLISTED"" 2 ""FUSED"" 0 ""REJECTED_UNAVAILABLE"" ;
VAL_ 569 DAS_rightFork 1 ""RIGHT_FORK_AVAILABLE"" 0 ""RIGHT_FORK_NONE"" 2 ""RIGHT_FORK_SELECTED"" 3 ""RIGHT_FORK_UNAVAILABLE"" ;
VAL_ 569 DAS_rightLineUsage 1 ""AVAILABLE"" 3 ""BLACKLISTED"" 2 ""FUSED"" 0 ""REJECTED_UNAVAILABLE"" ;
VAL_ 586 DAS_accSmartSpeedState 2 ""ACTIVE_INTEGRATING"" 1 ""ACTIVE_OFFRAMP"" 3 ""ACTIVE_ONRAMP"" 0 ""NOT_ACTIVE"" 5 ""OFFRAMP_DELAY"" 4 ""SET_SPEED_SET_REQUESTED"" 7 ""SNA"" ;
VAL_ 586 DAS_autosteerBottsDotsUsage 1 ""AVAILABLE"" 3 ""BLACKLISTED"" 2 ""FUSED"" 0 ""REJECTED_UNAVAILABLE"" ;
VAL_ 586 DAS_autosteerHPPUsage 1 ""AVAILABLE"" 3 ""BLACKLISTED"" 2 ""FUSED"" 0 ""REJECTED_UNAVAILABLE"" ;
VAL_ 586 DAS_autosteerHealthState 4 ""HEALTH_ABORTING"" 2 ""HEALTH_DEGRADED"" 5 ""HEALTH_FAULT"" 1 ""HEALTH_NOMINAL"" 3 ""HEALTH_SEVERELY_DEGRADED"" 0 ""HEALTH_UNAVAILABLE"" ;
VAL_ 586 DAS_autosteerModelUsage 1 ""AVAILABLE"" 3 ""BLACKLISTED"" 2 ""FUSED"" 0 ""REJECTED_UNAVAILABLE"" ;
VAL_ 586 DAS_autosteerNavigationUsage 1 ""AVAILABLE"" 3 ""BLACKLISTED"" 2 ""FUSED"" 0 ""REJECTED_UNAVAILABLE"" ;
VAL_ 586 DAS_autosteerVehiclesUsage 1 ""AVAILABLE"" 3 ""BLACKLISTED"" 2 ""FUSED"" 0 ""REJECTED_UNAVAILABLE"" ;
VAL_ 586 DAS_behaviorType 0 ""DAS_BEHAVIOR_INVALID"" 1 ""DAS_BEHAVIOR_IN_LANE"" 2 ""DAS_BEHAVIOR_LANE_CHANGE_LEFT"" 3 ""DAS_BEHAVIOR_LANE_CHANGE_RIGHT"" ;
VAL_ 586 DAS_lastAutosteerAbortReason 29 ""UI_ABORT_REASON_ACC_CANCEL"" 32 ""UI_ABORT_REASON_AEB"" 14 ""UI_ABORT_REASON_APP_ME_STATE_NOT_VISION"" 30 ""UI_ABORT_REASON_CAMERA_FAILSAFES"" 16 ""UI_ABORT_REASON_CAM_MSG_MIA"" 17 ""UI_ABORT_REASON_CAM_WATCHDOG"" 23 ""UI_ABORT_REASON_CID_SWITCH_DISABLED"" 21 ""UI_ABORT_REASON_COMPONENT_MIA"" 22 ""UI_ABORT_REASON_CRUISE_FAULT"" 24 ""UI_ABORT_REASON_DRIVING_OFF_NAV"" 20 ""UI_ABORT_REASON_EPAS_EAC_DENIED"" 28 ""UI_ABORT_REASON_EPAS_ERROR_CODE"" 26 ""UI_ABORT_REASON_FOLLOWER_OUTPUT_INVALID"" 0 ""UI_ABORT_REASON_HM_LANE_VIEW_RANGE"" 2 ""UI_ABORT_REASON_HM_STEERING_ERROR"" 1 ""UI_ABORT_REASON_HM_VIRTUAL_LANE_NO_INPUTS"" 15 ""UI_ABORT_REASON_ME_MAIN_STATE_NOT_VISION"" 31 ""UI_ABORT_REASON_NO_ABORT"" 27 ""UI_ABORT_REASON_PLANNER_OUTPUT_INVALID"" 33 ""UI_ABORT_REASON_SEATBELT_UNBUCKLED"" 19 ""UI_ABORT_REASON_SIDE_COLLISION_IMMINENT"" 18 ""UI_ABORT_REASON_TRAILER_MODE"" 34 ""UI_ABORT_REASON_USER_OVERRIDE_STRIKEOUT"" 25 ""UI_ABORT_REASON_VEHICLE_SPEED_ABOVE_MAX"" ;
VAL_ 586 DAS_lastLinePreferenceReason 1 ""AGREEMENT_WITH_NEIGHBOR_LANES"" 4 ""AVOID_ONCOMING_LANES"" 5 ""COUNTRY_DRIVING_SIDE"" 3 ""NAVIGATION_BRANCH"" 2 ""NEIGHBOR_LANE_PROBABILIY"" 15 ""NONE"" 0 ""OTHER_LANE_DISAGREES_WITH_MODEL"" ;
VAL_ 586 DAS_navAvailable 1 ""DAS_NAV_AVAILABLE"" 0 ""DAS_NAV_UNAVAILABLE"" ;
VAL_ 586 DAS_offsetSide 0 ""NO_OFFSET"" 3 ""OFFSET_BOTH_OBJECTS"" 2 ""OFFSET_LEFT_OBJECT"" 1 ""OFFSET_RIGHT_OBJECT"" ;
VAL_ 586 DAS_plannerState 0 ""TP_EXTSTATE_DISABLED"" 2 ""TP_EXTSTATE_FOLLOW"" 7 ""TP_EXTSTATE_LANECHANGE_ABORT"" 4 ""TP_EXTSTATE_LANECHANGE_IN_PROGRESS"" 3 ""TP_EXTSTATE_LANECHANGE_REQUESTED"" 6 ""TP_EXTSTATE_LANECHANGE_WAIT_FOR_FWD_OBSTACLE"" 5 ""TP_EXTSTATE_LANECHANGE_WAIT_FOR_SIDE_OBSTACLE"" 1 ""TP_EXTSTATE_VL"" ;
VAL_ 586 DAS_rearLeftVehDetectedCurrent 1 ""VEHICLE_DETECTED"" 0 ""VEHICLE_NOT_DETECTED"" ;
VAL_ 586 DAS_rearLeftVehDetectedTrip 1 ""VEHICLE_DETECTED"" 0 ""VEHICLE_NOT_DETECTED"" ;
VAL_ 586 DAS_rearRightVehDetectedTrip 1 ""VEHICLE_DETECTED"" 0 ""VEHICLE_NOT_DETECTED"" ;
VAL_ 586 DAS_rearVehDetectedThisCycle 1 ""VEHICLE_DETECTED"" 0 ""VEHICLE_NOT_DETECTED"" ;
VAL_ 586 DAS_roadSurfaceType 2 ""ROAD_SURFACE_ENHANCED"" 1 ""ROAD_SURFACE_NORMAL"" 0 ""ROAD_SURFACE_SNA"" ;
VAL_ 586 DAS_ulcInProgress 1 ""ULC_ACTIVE"" 0 ""ULC_INACTIVE"" ;
VAL_ 586 DAS_ulcType 1 ""ULC_TYPE_NAV"" 0 ""ULC_TYPE_NONE"" 2 ""ULC_TYPE_SPEED"" ;
VAL_ 605 CP_UHF_controlState 3 ""CP_UHF_CALIBRATE"" 6 ""CP_UHF_CHECK_RX"" 1 ""CP_UHF_CONFIG"" 10 ""CP_UHF_FAULT"" 8 ""CP_UHF_HANDLE_FOUND"" 2 ""CP_UHF_IDLE"" 0 ""CP_UHF_INIT"" 4 ""CP_UHF_PREPARE_RX"" 7 ""CP_UHF_READ_RXFIFO"" 5 ""CP_UHF_RX"" 9 ""CP_UHF_SLEEP"" ;
VAL_ 605 CP_chargeCablePresent 0 ""CABLE_NOT_PRESENT"" 1 ""CABLE_PRESENT"" ;
VAL_ 605 CP_chargeCableState 2 ""CHARGE_CABLE_CONNECTED"" 1 ""CHARGE_CABLE_NOT_CONNECTED"" 0 ""CHARGE_CABLE_UNKNOWN_SNA"" ;
VAL_ 605 CP_coldWeatherMode 1 ""CP_COLD_WEATHER_LATCH_MITIGATION"" 0 ""CP_COLD_WEATHER_NONE"" ;
VAL_ 605 CP_doorControlState 5 ""CP_doorClosing"" 1 ""CP_doorIdle"" 0 ""CP_doorInit"" 2 ""CP_doorOpenRequested"" 3 ""CP_doorOpening"" 6 ""CP_doorSenseClosed"" 4 ""CP_doorSenseOpen"" ;
VAL_ 605 CP_faultLineSensed 0 ""FAULT_LINE_CLEARED"" 1 ""FAULT_LINE_SET"" ;
VAL_ 605 CP_inductiveDoorState 7 ""CP_INDUCTIVE_DOOR_FAULT"" 0 ""CP_INDUCTIVE_DOOR_INIT"" 1 ""CP_INDUCTIVE_DOOR_INIT_FROM_CHARGE"" 2 ""CP_INDUCTIVE_DOOR_INIT_FROM_DRIVE"" 4 ""CP_INDUCTIVE_DOOR_NOT_PRESENT"" 6 ""CP_INDUCTIVE_DOOR_OFF_CHARGE"" 5 ""CP_INDUCTIVE_DOOR_OFF_DRIVE"" 3 ""CP_INDUCTIVE_DOOR_PRESENT"" ;
VAL_ 605 CP_inductiveSensorState 7 ""CP_INDUCTIVE_SENSOR_CONFIG"" 5 ""CP_INDUCTIVE_SENSOR_FAULT"" 0 ""CP_INDUCTIVE_SENSOR_INIT"" 3 ""CP_INDUCTIVE_SENSOR_PAUSE"" 1 ""CP_INDUCTIVE_SENSOR_POLL"" 6 ""CP_INDUCTIVE_SENSOR_RESET"" 2 ""CP_INDUCTIVE_SENSOR_SHUTDOWN"" 4 ""CP_INDUCTIVE_SENSOR_WAIT_FOR_INIT"" ;
VAL_ 605 CP_latch2ControlState 2 ""CP_latchDisengageRequested"" 4 ""CP_latchDisengaged"" 3 ""CP_latchDisengaging"" 5 ""CP_latchEngaging"" 1 ""CP_latchIdle"" 0 ""CP_latchInit"" ;
VAL_ 605 CP_latch2State 3 ""CP_LATCH_BLOCKING"" 1 ""CP_LATCH_DISENGAGED"" 2 ""CP_LATCH_ENGAGED"" 0 ""CP_LATCH_SNA"" ;
VAL_ 605 CP_latchControlState 2 ""CP_latchDisengageRequested"" 4 ""CP_latchDisengaged"" 3 ""CP_latchDisengaging"" 5 ""CP_latchEngaging"" 1 ""CP_latchIdle"" 0 ""CP_latchInit"" ;
VAL_ 605 CP_latchState 3 ""CP_LATCH_BLOCKING"" 1 ""CP_LATCH_DISENGAGED"" 2 ""CP_LATCH_ENGAGED"" 0 ""CP_LATCH_SNA"" ;
VAL_ 605 CP_ledColor 7 ""CP_LEDS_AMBER"" 3 ""CP_LEDS_BLUE"" 9 ""CP_LEDS_DEBUG"" 6 ""CP_LEDS_FLASHING_AMBER"" 10 ""CP_LEDS_FLASHING_BLUE"" 5 ""CP_LEDS_FLASHING_GREEN"" 2 ""CP_LEDS_GREEN"" 0 ""CP_LEDS_OFF"" 8 ""CP_LEDS_RAVE"" 1 ""CP_LEDS_RED"" 4 ""CP_LEDS_WHITE"" ;
VAL_ 605 CP_type 1 ""CP_TYPE_EURO_IEC"" 2 ""CP_TYPE_GB"" 3 ""CP_TYPE_IEC_CCS"" 0 ""CP_TYPE_US_TESLA"" ;
VAL_ 697 DAS_accState 1 ""ACC_CANCEL_CAMERA_BLIND"" 0 ""ACC_CANCEL_GENERIC"" 13 ""ACC_CANCEL_GENERIC_SILENT"" 14 ""ACC_CANCEL_OUT_OF_CALIBRATION"" 12 ""ACC_CANCEL_PATH_NOT_CLEAR"" 2 ""ACC_CANCEL_RADAR_BLIND"" 3 ""ACC_HOLD"" 4 ""ACC_ON"" 8 ""APC_ABORT"" 5 ""APC_BACKWARD"" 7 ""APC_COMPLETE"" 6 ""APC_FORWARD"" 9 ""APC_PAUSE"" 11 ""APC_SELFPARK_START"" 10 ""APC_UNPARK_COMPLETE"" 15 ""FAULT_SNA"" ;
VAL_ 697 DAS_accelMax 511 ""SNA"" ;
VAL_ 697 DAS_accelMin 511 ""SNA"" ;
VAL_ 697 DAS_aebEvent 1 ""AEB_ACTIVE"" 2 ""AEB_FAULT"" 0 ""AEB_NOT_ACTIVE"" 3 ""AEB_SNA"" ;
VAL_ 697 DAS_jerkMax 255 ""SNA"" ;
VAL_ 697 DAS_jerkMin 511 ""SNA"" ;
VAL_ 697 DAS_setSpeed 4095 ""SNA"" ;
VAL_ 723 UI_isSunUp 0 ""SUN_DOWN"" 3 ""SUN_SNA"" 1 ""SUN_UP"" ;
VAL_ 723 UI_solarAzimuthAngle 32768 ""SNA"" ;
VAL_ 723 UI_solarAzimuthAngleCarRef 255 ""SNA"" ;
VAL_ 723 UI_solarElevationAngle 127 ""SNA"" ;
VAL_ 777 DAS_objectId 3 ""CUTIN_VEHICLE"" 0 ""LEAD_VEHICLES"" 1 ""LEFT_VEHICLES"" 2 ""RIGHT_VEHICLES"" 4 ""ROAD_SIGN"" 5 ""VEHICLE_HEADINGS"" ;
VAL_ 777 DAS_cutinVehDx 255 ""SNA"" ;
VAL_ 777 DAS_cutinVehHeading 255 ""SNA"" ;
VAL_ 777 DAS_cutinVehId 127 ""SNA"" ;
VAL_ 777 DAS_cutinVehType 4 ""BICYCLE"" 2 ""CAR"" 3 ""MOTORCYCLE"" 5 ""PEDESTRIAN"" 1 ""TRUCK"" 0 ""UNKNOWN"" ;
VAL_ 777 DAS_cutinVehVxRel 15 ""SNA"" ;
VAL_ 777 DAS_leadVeh2Dx 255 ""SNA"" ;
VAL_ 777 DAS_leadVeh2Heading 255 ""SNA"" ;
VAL_ 777 DAS_leadVeh2Id 0 ""SNA"" ;
VAL_ 777 DAS_leadVeh2Type 4 ""BICYCLE"" 2 ""CAR"" 3 ""MOTORCYCLE"" 5 ""PEDESTRIAN"" 1 ""TRUCK"" 0 ""UNKNOWN"" ;
VAL_ 777 DAS_leadVeh2VxRel 15 ""SNA"" ;
VAL_ 777 DAS_leadVehDx 255 ""SNA"" ;
VAL_ 777 DAS_leadVehHeading 255 ""SNA"" ;
VAL_ 777 DAS_leadVehId 127 ""SNA"" ;
VAL_ 777 DAS_leadVehType 4 ""BICYCLE"" 2 ""CAR"" 6 ""IPSO"" 3 ""MOTORCYCLE"" 5 ""PEDESTRIAN"" 1 ""TRUCK"" 0 ""UNKNOWN"" ;
VAL_ 777 DAS_leadVehVxRel 15 ""SNA"" ;
VAL_ 777 DAS_leftVeh2Dx 255 ""SNA"" ;
VAL_ 777 DAS_leftVeh2Heading 255 ""SNA"" ;
VAL_ 777 DAS_leftVeh2Id 0 ""SNA"" ;
VAL_ 777 DAS_leftVeh2Type 4 ""BICYCLE"" 2 ""CAR"" 3 ""MOTORCYCLE"" 5 ""PEDESTRIAN"" 1 ""TRUCK"" 0 ""UNKNOWN"" ;
VAL_ 777 DAS_leftVeh2VxRel 15 ""SNA"" ;
VAL_ 777 DAS_leftVehDx 255 ""SNA"" ;
VAL_ 777 DAS_leftVehHeading 255 ""SNA"" ;
VAL_ 777 DAS_leftVehId 127 ""SNA"" ;
VAL_ 777 DAS_leftVehType 4 ""BICYCLE"" 2 ""CAR"" 3 ""MOTORCYCLE"" 5 ""PEDESTRIAN"" 1 ""TRUCK"" 0 ""UNKNOWN"" ;
VAL_ 777 DAS_leftVehVxRel 15 ""SNA"" ;
VAL_ 777 DAS_rightVeh2Dx 255 ""SNA"" ;
VAL_ 777 DAS_rightVeh2Heading 255 ""SNA"" ;
VAL_ 777 DAS_rightVeh2Id 0 ""SNA"" ;
VAL_ 777 DAS_rightVeh2Type 4 ""BICYCLE"" 2 ""CAR"" 3 ""MOTORCYCLE"" 5 ""PEDESTRIAN"" 1 ""TRUCK"" 0 ""UNKNOWN"" ;
VAL_ 777 DAS_rightVeh2VxRel 15 ""SNA"" ;
VAL_ 777 DAS_rightVehDx 255 ""SNA"" ;
VAL_ 777 DAS_rightVehHeading 255 ""SNA"" ;
VAL_ 777 DAS_rightVehId 127 ""SNA"" ;
VAL_ 777 DAS_rightVehType 4 ""BICYCLE"" 2 ""CAR"" 3 ""MOTORCYCLE"" 5 ""PEDESTRIAN"" 1 ""TRUCK"" 0 ""UNKNOWN"" ;
VAL_ 777 DAS_rightVehVxRel 15 ""SNA"" ;
VAL_ 777 DAS_roadSignArrow 0 ""CIRCLE"" 1 ""LEFT"" 2 ""RIGHT"" 3 ""STRAIGHT"" 4 ""UNKNOWN"" ;
VAL_ 777 DAS_roadSignColor 3 ""GREEN"" 0 ""NONE"" 1 ""RED"" 4 ""RED_YELLOW"" 2 ""YELLOW"" ;
VAL_ 777 DAS_roadSignId 255 ""SNA"" 0 ""STOP_SIGN"" 1 ""TRAFFIC_LIGHT"" ;
VAL_ 777 DAS_roadSignOrientation 2 ""HORIZONTAL_3_LIGHT"" 0 ""UNKNOWN"" 1 ""VERTICAL_3_LIGHT"" ;
VAL_ 777 DAS_roadSignSource 1 ""NAV"" 0 ""NONE"" 2 ""VISION"" ;
VAL_ 777 DAS_roadSignStopLineDist 1023 ""SNA"" ;
VAL_ 905 DAS_ACC_report 24 ""ACC_REPORT_BEHAVIOR_REPORT"" 23 ""ACC_REPORT_CAMERA_ONLY"" 21 ""ACC_REPORT_CIPV_CUTTING_OUT"" 10 ""ACC_REPORT_CSA"" 18 ""ACC_REPORT_FLEET_SPEEDS"" 13 ""ACC_REPORT_LC_EXTERNAL_STATE_ABORTED"" 12 ""ACC_REPORT_LC_EXTERNAL_STATE_ABORTING"" 14 ""ACC_REPORT_LC_EXTERNAL_STATE_ACTIVE_RESTRICTED"" 11 ""ACC_REPORT_LC_HANDS_ON_REQD_STRUCK_OUT"" 19 ""ACC_REPORT_MCVLR_DPP"" 20 ""ACC_REPORT_MCVLR_IN_PATH"" 22 ""ACC_REPORT_RADAR_OBJ_FIVE"" 15 ""ACC_REPORT_RADAR_OBJ_ONE"" 16 ""ACC_REPORT_RADAR_OBJ_TWO"" 1 ""ACC_REPORT_TARGET_CIPV"" 5 ""ACC_REPORT_TARGET_CUTIN"" 2 ""ACC_REPORT_TARGET_IN_FRONT_OF_CIPV"" 17 ""ACC_REPORT_TARGET_MCP"" 3 ""ACC_REPORT_TARGET_MCVL"" 4 ""ACC_REPORT_TARGET_MCVR"" 0 ""ACC_REPORT_TARGET_NONE"" 9 ""ACC_REPORT_TARGET_TYPE_FAULT"" 8 ""ACC_REPORT_TARGET_TYPE_IPSO"" 6 ""ACC_REPORT_TARGET_TYPE_STOP_SIGN"" 7 ""ACC_REPORT_TARGET_TYPE_TRAFFIC_LIGHT"" ;
VAL_ 905 DAS_accSpeedLimit 0 ""NONE"" 1023 ""SNA"" ;
VAL_ 905 DAS_activationFailureStatus 1 ""LC_ACTIVATION_FAILED_1"" 2 ""LC_ACTIVATION_FAILED_2"" 0 ""LC_ACTIVATION_IDLE"" ;
VAL_ 905 DAS_csaState 1 ""CSA_EXTERNAL_STATE_AVAILABLE"" 2 ""CSA_EXTERNAL_STATE_ENABLE"" 3 ""CSA_EXTERNAL_STATE_HOLD"" 0 ""CSA_EXTERNAL_STATE_UNAVAILABLE"" ;
VAL_ 905 DAS_driverInteractionLevel 2 ""CONTINUED_DRIVER_NOT_INTERACTING"" 0 ""DRIVER_INTERACTING"" 1 ""DRIVER_NOT_INTERACTING"" ;
VAL_ 905 DAS_longCollisionWarning 3 ""FCM_LONG_COLLISION_WARNING_IPSO"" 0 ""FCM_LONG_COLLISION_WARNING_NONE"" 2 ""FCM_LONG_COLLISION_WARNING_PEDESTRIAN"" 15 ""FCM_LONG_COLLISION_WARNING_SNA"" 4 ""FCM_LONG_COLLISION_WARNING_STOPSIGN_STOPLINE"" 5 ""FCM_LONG_COLLISION_WARNING_TFL_STOPLINE"" 6 ""FCM_LONG_COLLISION_WARNING_VEHICLE_CIPV"" 12 ""FCM_LONG_COLLISION_WARNING_VEHICLE_CIPV2"" 7 ""FCM_LONG_COLLISION_WARNING_VEHICLE_CUTIN"" 8 ""FCM_LONG_COLLISION_WARNING_VEHICLE_MCVL"" 9 ""FCM_LONG_COLLISION_WARNING_VEHICLE_MCVL2"" 10 ""FCM_LONG_COLLISION_WARNING_VEHICLE_MCVR"" 11 ""FCM_LONG_COLLISION_WARNING_VEHICLE_MCVR2"" 1 ""FCM_LONG_COLLISION_WARNING_VEHICLE_UNKNOWN"" ;
VAL_ 905 DAS_pmmCameraFaultReason 1 ""PMM_CAMERA_BLOCKED_FRONT"" 2 ""PMM_CAMERA_INVALID_MIA"" 0 ""PMM_CAMERA_NO_FAULT"" ;
VAL_ 905 DAS_pmmLoggingRequest 0 ""FALSE"" 1 ""TRUE"" ;
VAL_ 905 DAS_pmmObstacleSeverity 6 ""PMM_ACCEL_LIMIT"" 3 ""PMM_BRAKE_REQUEST"" 5 ""PMM_CRASH_FRONT"" 4 ""PMM_CRASH_REAR"" 2 ""PMM_IMMINENT_FRONT"" 1 ""PMM_IMMINENT_REAR"" 0 ""PMM_NONE"" 7 ""PMM_SNA"" ;
VAL_ 905 DAS_pmmRadarFaultReason 1 ""PMM_RADAR_BLOCKED_FRONT"" 2 ""PMM_RADAR_INVALID_MIA"" 0 ""PMM_RADAR_NO_FAULT"" ;
VAL_ 905 DAS_pmmSysFaultReason 7 ""PMM_FAULT_BRAKE_PEDAL_INHIBIT"" 1 ""PMM_FAULT_DAS_DISABLED"" 5 ""PMM_FAULT_DISABLED_BY_USER"" 3 ""PMM_FAULT_DI_FAULT"" 0 ""PMM_FAULT_NONE"" 6 ""PMM_FAULT_ROAD_TYPE"" 2 ""PMM_FAULT_SPEED"" 4 ""PMM_FAULT_STEERING_ANGLE_RATE"" ;
VAL_ 905 DAS_pmmUltrasonicsFaultReason 3 ""PMM_ULTRASONICS_BLOCKED_BOTH"" 1 ""PMM_ULTRASONICS_BLOCKED_FRONT"" 2 ""PMM_ULTRASONICS_BLOCKED_REAR"" 4 ""PMM_ULTRASONICS_INVALID_MIA"" 0 ""PMM_ULTRASONICS_NO_FAULT"" ;
VAL_ 905 DAS_ppOffsetDesiredRamp 128 ""PP_NO_OFFSET"" ;
VAL_ 905 DAS_radarTelemetry 0 ""RADAR_TELEMETRY_IDLE"" 1 ""RADAR_TELEMETRY_NORMAL"" 2 ""RADAR_TELEMETRY_URGENT"" ;
VAL_ 905 DAS_robState 2 ""ROB_STATE_ACTIVE"" 0 ""ROB_STATE_INHIBITED"" 3 ""ROB_STATE_MAPLESS"" 1 ""ROB_STATE_MEASURE"" ;
VAL_ 921 DAS_autoLaneChangeState 19 ""ALC_ABORT_BLINKER_TURNED_OFF"" 18 ""ALC_ABORT_LC_HEALTH_BAD"" 30 ""ALC_ABORT_MISSION_PLAN_INVALID"" 20 ""ALC_ABORT_OTHER_REASON"" 17 ""ALC_ABORT_POOR_VIEW_RANGE"" 15 ""ALC_ABORT_SIDE_OBSTACLE_PRESENT_L"" 16 ""ALC_ABORT_SIDE_OBSTACLE_PRESENT_R"" 29 ""ALC_ABORT_TIMEOUT"" 8 ""ALC_AVAILABLE_BOTH"" 6 ""ALC_AVAILABLE_ONLY_L"" 7 ""ALC_AVAILABLE_ONLY_R"" 26 ""ALC_BLOCKED_LANE_TYPE_L"" 27 ""ALC_BLOCKED_LANE_TYPE_R"" 23 ""ALC_BLOCKED_VEH_TTC_AND_USS_L"" 25 ""ALC_BLOCKED_VEH_TTC_AND_USS_R"" 22 ""ALC_BLOCKED_VEH_TTC_L"" 24 ""ALC_BLOCKED_VEH_TTC_R"" 9 ""ALC_IN_PROGRESS_L"" 10 ""ALC_IN_PROGRESS_R"" 31 ""ALC_SNA"" 0 ""ALC_UNAVAILABLE_DISABLED"" 4 ""ALC_UNAVAILABLE_EXITING_HIGHWAY"" 1 ""ALC_UNAVAILABLE_NO_LANES"" 21 ""ALC_UNAVAILABLE_SOLID_LANE_LINE"" 2 ""ALC_UNAVAILABLE_SONICS_INVALID"" 3 ""ALC_UNAVAILABLE_TP_FOLLOW"" 5 ""ALC_UNAVAILABLE_VEHICLE_SPEED"" 13 ""ALC_WAITING_FOR_FWD_OBST_TO_PASS_L"" 14 ""ALC_WAITING_FOR_FWD_OBST_TO_PASS_R"" 11 ""ALC_WAITING_FOR_SIDE_OBST_TO_PASS_L"" 12 ""ALC_WAITING_FOR_SIDE_OBST_TO_PASS_R"" 28 ""ALC_WAITING_HANDS_ON"" ;
VAL_ 921 DAS_autoparkReady 1 ""AUTOPARK_READY"" 0 ""AUTOPARK_UNAVAILABLE"" ;
VAL_ 921 DAS_autopilotHandsOnState 0 ""LC_HANDS_ON_NOT_REQD"" 4 ""LC_HANDS_ON_REQD_CHIME_1"" 5 ""LC_HANDS_ON_REQD_CHIME_2"" 1 ""LC_HANDS_ON_REQD_DETECTED"" 9 ""LC_HANDS_ON_REQD_ESCALATED_CHIME_1"" 10 ""LC_HANDS_ON_REQD_ESCALATED_CHIME_2"" 2 ""LC_HANDS_ON_REQD_NOT_DETECTED"" 6 ""LC_HANDS_ON_REQD_SLOWING"" 7 ""LC_HANDS_ON_REQD_STRUCK_OUT"" 3 ""LC_HANDS_ON_REQD_VISUAL"" 15 ""LC_HANDS_ON_SNA"" 8 ""LC_HANDS_ON_SUSPENDED"" ;
VAL_ 921 DAS_autopilotState 9 ""ABORTED"" 8 ""ABORTING"" 5 ""ACTIVE_NAV"" 3 ""ACTIVE_NOMINAL"" 4 ""ACTIVE_RESTRICTED"" 2 ""AVAILABLE"" 0 ""DISABLED"" 14 ""FAULT"" 15 ""SNA"" 1 ""UNAVAILABLE"" ;
VAL_ 921 DAS_blindSpotRearLeft 0 ""NO_WARNING"" 3 ""SNA"" 1 ""WARNING_LEVEL_1"" 2 ""WARNING_LEVEL_2"" ;
VAL_ 921 DAS_blindSpotRearRight 0 ""NO_WARNING"" 3 ""SNA"" 1 ""WARNING_LEVEL_1"" 2 ""WARNING_LEVEL_2"" ;
VAL_ 921 DAS_fleetSpeedState 2 ""FLEETSPEED_ACTIVE"" 1 ""FLEETSPEED_AVAILABLE"" 3 ""FLEETSPEED_HOLD"" 0 ""FLEETSPEED_UNAVAILABLE"" ;
VAL_ 921 DAS_forwardCollisionWarning 1 ""FORWARD_COLLISION_WARNING"" 0 ""NONE"" 3 ""SNA"" ;
VAL_ 921 DAS_fusedSpeedLimit 31 ""NONE"" 0 ""UNKNOWN_SNA"" ;
VAL_ 921 DAS_heaterState 0 ""HEATER_OFF_SNA"" 1 ""HEATER_ON"" ;
VAL_ 921 DAS_laneDepartureWarning 1 ""LEFT_WARNING"" 3 ""LEFT_WARNING_SEVERE"" 0 ""NONE"" 2 ""RIGHT_WARNING"" 4 ""RIGHT_WARNING_SEVERE"" 5 ""SNA"" ;
VAL_ 921 DAS_lssState 6 ""LSS_STATE_ABORT"" 5 ""LSS_STATE_BLINDSPOT"" 3 ""LSS_STATE_ELK"" 0 ""LSS_STATE_FAULT"" 1 ""LSS_STATE_LDW"" 2 ""LSS_STATE_LKA"" 4 ""LSS_STATE_MONITOR"" 7 ""LSS_STATE_OFF"" ;
VAL_ 921 DAS_sideCollisionAvoid 1 ""AVOID_LEFT"" 2 ""AVOID_RIGHT"" 0 ""NONE"" 3 ""SNA"" ;
VAL_ 921 DAS_sideCollisionInhibit 1 ""INHIBIT"" 0 ""NO_INHIBIT"" ;
VAL_ 921 DAS_sideCollisionWarning 0 ""NONE"" 1 ""WARN_LEFT"" 3 ""WARN_LEFT_RIGHT"" 2 ""WARN_RIGHT"" ;
VAL_ 921 DAS_suppressSpeedWarning 0 ""Do_Not_Suppress"" 1 ""Suppress_Speed_Warning"" ;
VAL_ 921 DAS_visionOnlySpeedLimit 31 ""NONE"" 0 ""UNKNOWN_SNA"" ;
VAL_ 925 IBST_driverBrakeApply 1 ""BRAKES_NOT_APPLIED"" 2 ""DRIVER_APPLYING_BRAKES"" 3 ""FAULT"" 0 ""NOT_INIT_OR_OFF"" ;
VAL_ 925 IBST_iBoosterStatus 4 ""IBOOSTER_ACTIVE_GOOD_CHECK"" 6 ""IBOOSTER_ACTUATION"" 3 ""IBOOSTER_DIAGNOSTIC"" 2 ""IBOOSTER_FAILURE"" 1 ""IBOOSTER_INIT"" 0 ""IBOOSTER_OFF"" 5 ""IBOOSTER_READY"" ;
VAL_ 925 IBST_internalState 4 ""DIAGNOSTIC"" 3 ""EXTERNAL_BRAKE_REQUEST"" 2 ""LOCAL_BRAKE_REQUEST"" 0 ""NO_MODE_ACTIVE"" 6 ""POST_DRIVE_CHECK"" 1 ""PRE_DRIVE_CHECK"" 5 ""TRANSITION_TO_IDLE"" ;
VAL_ 929 VCFRONT_12vStatusForDrive 2 ""EXIT_DRIVE_REQUESTED_12V"" 0 ""NOT_READY_FOR_DRIVE_12V"" 1 ""READY_FOR_DRIVE_12V"" ;
VAL_ 929 VCFRONT_2RowCenterUnbuckled 0 ""CHIME_NONE"" 1 ""CHIME_OCCUPIED_AND_UNBUCKLED"" 2 ""CHIME_SNA"" ;
VAL_ 929 VCFRONT_2RowLeftUnbuckled 0 ""CHIME_NONE"" 1 ""CHIME_OCCUPIED_AND_UNBUCKLED"" 2 ""CHIME_SNA"" ;
VAL_ 929 VCFRONT_2RowRightUnbuckled 0 ""CHIME_NONE"" 1 ""CHIME_OCCUPIED_AND_UNBUCKLED"" 2 ""CHIME_SNA"" ;
VAL_ 929 VCFRONT_APGlassHeaterState 4 ""HEATER_STATE_FAULT"" 2 ""HEATER_STATE_OFF"" 3 ""HEATER_STATE_OFF_UNAVAILABLE"" 1 ""HEATER_STATE_ON"" 0 ""HEATER_STATE_SNA"" ;
VAL_ 929 VCFRONT_diPowerOnState 0 ""DI_POWERED_OFF"" 3 ""DI_POWERED_ON_FOR_DRIVE"" 2 ""DI_POWERED_ON_FOR_STATIONARY_HEAT"" 1 ""DI_POWERED_ON_FOR_SUMMON"" 4 ""DI_POWER_GOING_DOWN"" ;
VAL_ 929 VCFRONT_driverBuckleStatus 1 ""BUCKLED"" 0 ""UNBUCKLED"" ;
VAL_ 929 VCFRONT_driverDoorStatus 1 ""DOOR_CLOSED"" 0 ""DOOR_OPEN"" ;
VAL_ 929 VCFRONT_driverUnbuckled 0 ""CHIME_NONE"" 1 ""CHIME_OCCUPIED_AND_UNBUCKLED"" 2 ""CHIME_SNA"" ;
VAL_ 929 VCFRONT_passengerUnbuckled 0 ""CHIME_NONE"" 1 ""CHIME_OCCUPIED_AND_UNBUCKLED"" 2 ""CHIME_SNA"" ;
VAL_ 929 VCFRONT_pcsEFuseVoltage 1023 ""SNA"" ;
VAL_ 929 VCFRONT_thermalSystemType 1 ""HEAT_PUMP_THERMAL_SYSTEM"" 0 ""LEGACY_THERMAL_SYSTEM"" ;
VAL_ 985 UI_conditionalSpeedLimit 31 ""SNA"" ;
VAL_ 985 UI_mapSpeedLimitUnits 1 ""KPH"" 0 ""MPH"" ;
VAL_ 985 UI_userSpeedOffsetUnits 1 ""KPH"" 0 ""MPH"" ;
VAL_ 994 VCLEFT_FLMapLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_FRMapLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_RLMapLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_RRMapLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_brakeLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_brakeTrailerLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_dynamicBrakeLightStatus 2 ""DYNAMIC_BRAKE_LIGHT_ACTIVE_HIGH"" 1 ""DYNAMIC_BRAKE_LIGHT_ACTIVE_LOW"" 0 ""DYNAMIC_BRAKE_LIGHT_OFF"" ;
VAL_ 994 VCLEFT_fogTrailerLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_frontRideHeight 128 ""SNA"" ;
VAL_ 994 VCLEFT_leftTurnTrailerLightStatu 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_rearRideHeight 128 ""SNA"" ;
VAL_ 994 VCLEFT_reverseTrailerLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_rightTrnTrailerLightStatu 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_tailLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_tailTrailerLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 994 VCLEFT_trailerDetected 2 ""TRAILER_LIGHT_DETECTION_DETECTED"" 1 ""TRAILER_LIGHT_DETECTION_FAULT"" 3 ""TRAILER_LIGHT_DETECTION_NOT_DETECTED"" 0 ""TRAILER_LIGHT_DETECTION_SNA"" ;
VAL_ 994 VCLEFT_turnSignalStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1001 DAS_hazardLightRequest 0 ""DAS_REQUEST_HAZARDS_OFF"" 1 ""DAS_REQUEST_HAZARDS_ON"" 3 ""DAS_REQUEST_HAZARDS_SNA"" 2 ""DAS_REQUEST_HAZARDS_UNUSED"" ;
VAL_ 1001 DAS_headlightRequest 3 ""DAS_HEADLIGHT_REQUEST_INVALID"" 0 ""DAS_HEADLIGHT_REQUEST_OFF"" 1 ""DAS_HEADLIGHT_REQUEST_ON"" ;
VAL_ 1001 DAS_heaterRequest 1 ""DAS_HEATER_OFF"" 2 ""DAS_HEATER_ON"" 0 ""DAS_HEATER_SNA"" ;
VAL_ 1001 DAS_highLowBeamDecision 1 ""DAS_HIGH_BEAM_OFF"" 2 ""DAS_HIGH_BEAM_ON"" 3 ""DAS_HIGH_BEAM_SNA"" 0 ""DAS_HIGH_BEAM_UNDECIDED"" ;
VAL_ 1001 DAS_highLowBeamOffReason 3 ""HIGH_BEAM_OFF_REASON_AMBIENT_LIGHT"" 4 ""HIGH_BEAM_OFF_REASON_HEAD_LIGHT"" 2 ""HIGH_BEAM_OFF_REASON_MOVING_RADAR_TARGET"" 1 ""HIGH_BEAM_OFF_REASON_MOVING_VISION_TARGET"" 5 ""HIGH_BEAM_OFF_REASON_SNA"" 0 ""HIGH_BEAM_ON"" ;
VAL_ 1001 DAS_turnIndicatorRequest 3 ""DAS_TURN_INDICATOR_CANCEL"" 4 ""DAS_TURN_INDICATOR_DEFER"" 1 ""DAS_TURN_INDICATOR_LEFT"" 0 ""DAS_TURN_INDICATOR_NONE"" 2 ""DAS_TURN_INDICATOR_RIGHT"" ;
VAL_ 1001 DAS_turnIndicatorRequestReason 8 ""DAS_ACTIVE_COMMANDED_LANE_CHANGE"" 3 ""DAS_ACTIVE_FORK"" 9 ""DAS_ACTIVE_INTERSECTION"" 6 ""DAS_ACTIVE_MERGE"" 1 ""DAS_ACTIVE_NAV_LANE_CHANGE"" 2 ""DAS_ACTIVE_SPEED_LANE_CHANGE"" 11 ""DAS_ACTIVE_SUMMMON"" 5 ""DAS_CANCEL_FORK"" 10 ""DAS_CANCEL_INTERSECTION"" 4 ""DAS_CANCEL_LANE_CHANGE"" 7 ""DAS_CANCEL_MERGE"" 12 ""DAS_CANCEL_SUMMMON"" 0 ""DAS_NONE"" ;
VAL_ 1001 DAS_wiperSpeed 1 ""DAS_WIPER_SPEED_1"" 10 ""DAS_WIPER_SPEED_10"" 11 ""DAS_WIPER_SPEED_11"" 12 ""DAS_WIPER_SPEED_12"" 13 ""DAS_WIPER_SPEED_13"" 14 ""DAS_WIPER_SPEED_14"" 2 ""DAS_WIPER_SPEED_2"" 3 ""DAS_WIPER_SPEED_3"" 4 ""DAS_WIPER_SPEED_4"" 5 ""DAS_WIPER_SPEED_5"" 6 ""DAS_WIPER_SPEED_6"" 7 ""DAS_WIPER_SPEED_7"" 8 ""DAS_WIPER_SPEED_8"" 9 ""DAS_WIPER_SPEED_9"" 15 ""DAS_WIPER_SPEED_INVALID"" 0 ""DAS_WIPER_SPEED_OFF"" ;
VAL_ 1001 DAS_mirrorFoldRequest 0 ""NONE"" 1 ""FOLD"" 2 ""UNFOLD"" 3 ""SNA"" ;
VAL_ 1011 UI_odometer 16777215 ""SNA"" ;
VAL_ 1013 VCFRONT_DRLLeftStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_DRLRightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_ambientLightingBrightnes 255 ""SNA"" ;
VAL_ 1013 VCFRONT_fogLeftStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_fogRightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_hazardLightRequest 1 ""HAZARD_REQUEST_BUTTON"" 6 ""HAZARD_REQUEST_CAR_ALARM"" 5 ""HAZARD_REQUEST_CRASH"" 7 ""HAZARD_REQUEST_DAS"" 2 ""HAZARD_REQUEST_LOCK"" 4 ""HAZARD_REQUEST_MISLOCK"" 0 ""HAZARD_REQUEST_NONE"" 8 ""HAZARD_REQUEST_UDS"" 3 ""HAZARD_REQUEST_UNLOCK"" ;
VAL_ 1013 VCFRONT_highBeamLeftStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_highBeamRightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_indicatorLeftRequest 2 ""TURN_SIGNAL_ACTIVE_HIGH"" 1 ""TURN_SIGNAL_ACTIVE_LOW"" 0 ""TURN_SIGNAL_OFF"" ;
VAL_ 1013 VCFRONT_indicatorRightRequest 2 ""TURN_SIGNAL_ACTIVE_HIGH"" 1 ""TURN_SIGNAL_ACTIVE_LOW"" 0 ""TURN_SIGNAL_OFF"" ;
VAL_ 1013 VCFRONT_lowBeamLeftStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_lowBeamRightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_parkLeftStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_parkRightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_sideMarkersStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_sideRepeaterLeftStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_sideRepeaterRightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_simLatchingStalk 0 ""SIMULATED_LATCHING_STALK_IDLE"" 1 ""SIMULATED_LATCHING_STALK_LEFT"" 2 ""SIMULATED_LATCHING_STALK_RIGHT"" 3 ""SIMULATED_LATCHING_STALK_SNA"" ;
VAL_ 1013 VCFRONT_switchLightingBrightness 255 ""SNA"" ;
VAL_ 1013 VCFRONT_turnSignalLeftStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1013 VCFRONT_turnSignalRightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1016 UI_accFollowDistanceSetting 0 ""DISTANCE_SETTING_1"" 1 ""DISTANCE_SETTING_2"" 2 ""DISTANCE_SETTING_3"" 3 ""DISTANCE_SETTING_4"" 4 ""DISTANCE_SETTING_5"" 5 ""DISTANCE_SETTING_6"" 6 ""DISTANCE_SETTING_7"" 7 ""DISTANCE_SETTING_SNA"" ;
VAL_ 1016 UI_autopilotControlRequest 0 ""LEGACY_LAT_CTRL"" 1 ""NEXT_GEN_CTRL"" ;
VAL_ 1016 UI_curvSpeedAdaptDisable 1 ""CSA_OFF"" 0 ""CSA_ON"" ;
VAL_ 1016 UI_curvatureDatabaseOnly 0 ""OFF"" 1 ""ON"" ;
VAL_ 1016 UI_driveOnMapsEnable 0 ""DOM_OFF"" 1 ""DOM_ON"" ;
VAL_ 1016 UI_drivingSide 0 ""DRIVING_SIDE_LEFT"" 1 ""DRIVING_SIDE_RIGHT"" 2 ""DRIVING_SIDE_UNKNOWN"" ;
VAL_ 1016 UI_exceptionListEnable 0 ""EXCEPTION_LIST_OFF"" 1 ""EXCEPTION_LIST_ON"" ;
VAL_ 1016 UI_followNavRouteEnable 0 ""NAV_ROUTE_OFF"" 1 ""NAV_ROUTE_ON"" ;
VAL_ 1016 UI_fuseHPPDisable 1 ""FUSE_HPP_OFF"" 0 ""FUSE_HPP_ON"" ;
VAL_ 1016 UI_fuseLanesDisable 1 ""FUSE_LANES_OFF"" 0 ""FUSE_LANES_ON"" ;
VAL_ 1016 UI_fuseVehiclesDisable 1 ""FUSE_VEH_OFF"" 0 ""FUSE_VEH_ON"" ;
VAL_ 1016 UI_handsOnRequirementDisable 1 ""HANDS_ON_REQ_OFF"" 0 ""HANDS_ON_REQ_ON"" ;
VAL_ 1016 UI_lssElkEnabled 0 ""ELK_OFF"" 1 ""ELK_ON"" ;
VAL_ 1016 UI_lssLdwEnabled 0 ""LDW_OFF"" 1 ""LDW_ON"" ;
VAL_ 1016 UI_lssLkaEnabled 0 ""LKA_OFF"" 1 ""LKA_ON"" ;
VAL_ 1016 UI_roadCheckDisable 1 ""RC_OFF"" 0 ""RC_ON"" ;
VAL_ 1016 UI_selfParkRequest 3 ""ABORT"" 9 ""AUTO_SUMMON_CANCEL"" 7 ""AUTO_SUMMON_FORWARD"" 10 ""AUTO_SUMMON_PRIMED"" 8 ""AUTO_SUMMON_REVERSE"" 0 ""NONE"" 5 ""PAUSE"" 4 ""PRIME"" 6 ""RESUME"" 1 ""SELF_PARK_FORWARD"" 2 ""SELF_PARK_REVERSE"" 11 ""SMART_SUMMON"" 12 ""SMART_SUMMON_NO_OP"" 15 ""SNA"" ;
VAL_ 1016 UI_smartSummonType 1 ""FIND_ME"" 0 ""PIN_DROP"" 2 ""SMART_AUTOPARK"" ;
VAL_ 1016 UI_source3D 2 ""XYZ_PREDICTION"" 0 ""Z_FROM_MAP"" 1 ""Z_FROM_PATH_PREDICTION"" ;
VAL_ 1016 UI_summonEntryType 3 ""SNA"" 0 ""STRAIGHT"" 2 ""TURN_LEFT"" 1 ""TURN_RIGHT"" ;
VAL_ 1016 UI_summonExitType 3 ""SNA"" 0 ""STRAIGHT"" 2 ""TURN_LEFT"" 1 ""TURN_RIGHT"" ;
VAL_ 1016 UI_summonReverseDist 63 ""SNA"" ;
VAL_ 1016 UI_ulcBlindSpotConfig 1 ""AGGRESSIVE"" 2 ""MAD_MAX"" 0 ""STANDARD"" ;
VAL_ 1016 UI_ulcSpeedConfig 2 ""SPEED_BASED_ULC_AVERAGE"" 0 ""SPEED_BASED_ULC_DISABLED"" 3 ""SPEED_BASED_ULC_MAD_MAX"" 1 ""SPEED_BASED_ULC_MILD"" ;
VAL_ 1016 UI_visionSpeedType 0 ""VISION_SPEED_DISABLED"" 1 ""VISION_SPEED_ONE_SECOND"" 3 ""VISION_SPEED_OPTIMIZED"" 2 ""VISION_SPEED_TWO_SECOND"" ;
VAL_ 1021 UI_autopilotControlIndex 0 ""AUTOPILOT_CONTROL_0"" 1 ""AUTOPILOT_CONTROL_1"" 2 ""AUTOPILOT_CONTROL_2"" 3 ""AUTOPILOT_CONTROL_3"" 4 ""AUTOPILOT_CONTROL_4"" 5 ""AUTOPILOT_CONTROL_5"" 6 ""AUTOPILOT_CONTROL_6"" 7 ""AUTOPILOT_CONTROL_7"" ;
VAL_ 1021 UI_apmv3Branch 5 ""DEMO"" 2 ""DEV"" 4 ""EAP"" 0 ""LIVE"" 1 ""STAGE"" 3 ""STAGE2"" ;
VAL_ 1021 UI_blindspotDistance 1 ""BLINDSPOT_DISTANCE_0P5_M"" 2 ""BLINDSPOT_DISTANCE_1_M"" 3 ""BLINDSPOT_DISTANCE_2_M"" 4 ""BLINDSPOT_DISTANCE_4_M"" 0 ""BLINDSPOT_DISTANCE_DEFAULT"" 5 ""BLINDSPOT_DISTANCE_OFF"" ;
VAL_ 1021 UI_blindspotMinSpeed 2 ""BLINDSPOT_MIN_SPEED_10_KPH"" 3 ""BLINDSPOT_MIN_SPEED_15_KPH"" 4 ""BLINDSPOT_MIN_SPEED_20_KPH"" 5 ""BLINDSPOT_MIN_SPEED_25_KPH"" 6 ""BLINDSPOT_MIN_SPEED_30_KPH"" 7 ""BLINDSPOT_MIN_SPEED_35_KPH"" 8 ""BLINDSPOT_MIN_SPEED_40_KPH"" 9 ""BLINDSPOT_MIN_SPEED_45_KPH"" 1 ""BLINDSPOT_MIN_SPEED_5_KPH"" 0 ""BLINDSPOT_MIN_SPEED_DEFAULT"" 10 ""BLINDSPOT_MIN_SPEED_OFF"" ;
VAL_ 1021 UI_blindspotTTC 1 ""BLINDSPOT_TTC_0P5_S"" 2 ""BLINDSPOT_TTC_1_S"" 3 ""BLINDSPOT_TTC_2_S"" 5 ""BLINDSPOT_TTC_3_S"" 4 ""BLINDSPOT_TTC_4_S"" 6 ""BLINDSPOT_TTC_5_S"" 0 ""BLINDSPOT_TTC_DEFAULT"" 7 ""BLINDSPOT_TTC_OFF"" ;
VAL_ 1021 UI_donAlcProgGoreAbortThres 1 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P00"" 2 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P05"" 3 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P10"" 4 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P15"" 5 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P20"" 6 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P25"" 7 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P30"" 8 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P35"" 9 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_0P40"" 0 ""DON_ALC_PROGRESS_GORE_ABORT_THRESHOLD_DEFAULT"" ;
VAL_ 1021 UI_donDisableAutoWiperDuration 5 ""DON_DISABLE_AUTO_WIPER_DURATION_120_S"" 2 ""DON_DISABLE_AUTO_WIPER_DURATION_15_S"" 3 ""DON_DISABLE_AUTO_WIPER_DURATION_30_S"" 1 ""DON_DISABLE_AUTO_WIPER_DURATION_5_S"" 4 ""DON_DISABLE_AUTO_WIPER_DURATION_60_S"" 0 ""DON_DISABLE_AUTO_WIPER_DURATION_DEFAULT"" 6 ""DON_DISABLE_AUTO_WIPER_DURATION_OFF"" ;
VAL_ 1021 UI_donDisableCutin 0 ""DON_DISABLE_CUTIN_OFF"" 1 ""DON_DISABLE_CUTIN_ON"" ;
VAL_ 1021 UI_donDisableOnAutoWiperSpeed 1 ""DAS_WIPER_SPEED_1"" 10 ""DAS_WIPER_SPEED_10"" 11 ""DAS_WIPER_SPEED_11"" 12 ""DAS_WIPER_SPEED_12"" 13 ""DAS_WIPER_SPEED_13"" 14 ""DAS_WIPER_SPEED_14"" 2 ""DAS_WIPER_SPEED_2"" 3 ""DAS_WIPER_SPEED_3"" 4 ""DAS_WIPER_SPEED_4"" 5 ""DAS_WIPER_SPEED_5"" 6 ""DAS_WIPER_SPEED_6"" 7 ""DAS_WIPER_SPEED_7"" 8 ""DAS_WIPER_SPEED_8"" 9 ""DAS_WIPER_SPEED_9"" 15 ""DAS_WIPER_SPEED_INVALID"" 0 ""DAS_WIPER_SPEED_OFF"" ;
VAL_ 1021 UI_donMinGoreWidthForAbortMap 2 ""DON_MIN_GORE_WIDTH_FOR_ABORT_0P5_M"" 1 ""DON_MIN_GORE_WIDTH_FOR_ABORT_0_M"" 4 ""DON_MIN_GORE_WIDTH_FOR_ABORT_1P5_M"" 3 ""DON_MIN_GORE_WIDTH_FOR_ABORT_1_M"" 6 ""DON_MIN_GORE_WIDTH_FOR_ABORT_2P5_M"" 5 ""DON_MIN_GORE_WIDTH_FOR_ABORT_2_M"" 8 ""DON_MIN_GORE_WIDTH_FOR_ABORT_3P5_M"" 7 ""DON_MIN_GORE_WIDTH_FOR_ABORT_3_M"" 10 ""DON_MIN_GORE_WIDTH_FOR_ABORT_4P5_M"" 9 ""DON_MIN_GORE_WIDTH_FOR_ABORT_4_M"" 11 ""DON_MIN_GORE_WIDTH_FOR_ABORT_5_M"" 0 ""DON_MIN_GORE_WIDTH_FOR_ABORT_DEFAULT"" ;
VAL_ 1021 UI_donMinGoreWidthForAbortNotMap 2 ""DON_MIN_GORE_WIDTH_FOR_ABORT_0P5_M"" 1 ""DON_MIN_GORE_WIDTH_FOR_ABORT_0_M"" 4 ""DON_MIN_GORE_WIDTH_FOR_ABORT_1P5_M"" 3 ""DON_MIN_GORE_WIDTH_FOR_ABORT_1_M"" 6 ""DON_MIN_GORE_WIDTH_FOR_ABORT_2P5_M"" 5 ""DON_MIN_GORE_WIDTH_FOR_ABORT_2_M"" 8 ""DON_MIN_GORE_WIDTH_FOR_ABORT_3P5_M"" 7 ""DON_MIN_GORE_WIDTH_FOR_ABORT_3_M"" 10 ""DON_MIN_GORE_WIDTH_FOR_ABORT_4P5_M"" 9 ""DON_MIN_GORE_WIDTH_FOR_ABORT_4_M"" 11 ""DON_MIN_GORE_WIDTH_FOR_ABORT_5_M"" 0 ""DON_MIN_GORE_WIDTH_FOR_ABORT_DEFAULT"" ;
VAL_ 1021 UI_donStopEndOfRampBuffer 1 ""DON_STOP_END_OF_RAMP_BUFFER_15_M"" 2 ""DON_STOP_END_OF_RAMP_BUFFER_30_M"" 3 ""DON_STOP_END_OF_RAMP_BUFFER_45_M"" 0 ""DON_STOP_END_OF_RAMP_BUFFER_DEFAULT"" 4 ""DON_STOP_END_OF_RAMP_BUFFER_OFF"" ;
VAL_ 1021 UI_homelinkNearby 1 ""HOMELINK_NEARBY"" 0 ""HOMELINK_NOT_NEARBY"" ;
VAL_ 1021 UI_hovEnabled 0 ""HOV_OFF"" 1 ""HOV_ON"" ;
VAL_ 615 DI_massConfidence 1 ""MASS_CONFIDED"" 0 ""MASS_NOT_CONFIDED"" ;
VAL_ 615 DI_relativeTireTreadDepth 32 ""SNA"" ;
VAL_ 615 DI_tireFitment 3 ""FITMENT_SNA"" 0 ""FITMENT_SQUARE"" 1 ""FITMENT_STAGGERED"" ;
VAL_ 615 DI_trailerDetected 1 ""TRAILER_DETECTED"" 0 ""TRAILER_NOT_DETECTED"" ;
VAL_ 642 VCLEFT_blowerIndex 0 ""HVAC_FEEDBACK_SIGNALS"" 1 ""HVAC_VARS"" ;
VAL_ 642 VCLEFT_hvacBlowerCBCCtrlState 4 ""COUNT"" 0 ""ERROR"" 1 ""IDLE"" 2 ""OFFLINE"" 3 ""ONLINE"" ;
VAL_ 642 VCLEFT_hvacBlowerCBCEstState 14 ""COUNT"" 0 ""ERROR"" 1 ""IDLE"" 5 ""IDRATED"" 10 ""INDUCTANCE_EST"" 9 ""LOCKROTOR"" 12 ""MOTOR_IDENTIFIED"" 13 ""ONLINE"" 8 ""RAMPDOWN"" 4 ""RAMPUP"" 7 ""RATEDFLUX"" 6 ""RATEDFLUX_OL"" 11 ""ROTOR_RESISTANCE"" 2 ""ROVERL"" 3 ""RS"" ;
VAL_ 642 VCLEFT_hvacBlowerFETTemp 127 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerIPhase0 255 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerIPhase1 255 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerIPhase2 255 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerITerm 127 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerOutputDuty 127 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerRPMTarget 1023 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerRs 255 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlowerTorque 1023 ""SNA"" ;
VAL_ 642 VCLEFT_hvacBlower_IO_CBC_Status 0 ""IDLE"" 1 ""RX"" ;
VAL_ 755 UI_hvacReqACDisable 0 ""AUTO"" 1 ""OFF"" 2 ""ON"" ;
VAL_ 755 UI_hvacReqAirDistributionMode 0 ""AUTO"" 4 ""MANUAL_DEFROST"" 5 ""MANUAL_DEFROST_FLOOR"" 6 ""MANUAL_DEFROST_PANEL"" 7 ""MANUAL_DEFROST_PANEL_FLOOR"" 1 ""MANUAL_FLOOR"" 2 ""MANUAL_PANEL"" 3 ""MANUAL_PANEL_FLOOR"" ;
VAL_ 755 UI_hvacReqBlowerSegment 1 ""1"" 10 ""10"" 2 ""2"" 3 ""3"" 4 ""4"" 5 ""5"" 6 ""6"" 7 ""7"" 8 ""8"" 9 ""9"" 11 ""AUTO"" 0 ""OFF"" ;
VAL_ 755 UI_hvacReqKeepClimateOn 2 ""KEEP_CLIMATE_ON_REQ_DOG"" 0 ""KEEP_CLIMATE_ON_REQ_OFF"" 1 ""KEEP_CLIMATE_ON_REQ_ON"" 3 ""KEEP_CLIMATE_ON_REQ_PARTY"" ;
VAL_ 755 UI_hvacReqManualDefogState 1 ""DEFOG"" 2 ""DEFROST"" 0 ""NONE"" ;
VAL_ 755 UI_hvacReqRecirc 0 ""AUTO"" 2 ""FRESH"" 1 ""RECIRC"" ;
VAL_ 755 UI_hvacReqSecondRowState 0 ""AUTO"" 4 ""HIGH"" 2 ""LOW"" 3 ""MED"" 1 ""OFF"" ;
VAL_ 755 UI_hvacReqTempSetpointLeft 26 ""HI"" 0 ""LO"" ;
VAL_ 755 UI_hvacReqTempSetpointRight 26 ""HI"" 0 ""LO"" ;
VAL_ 755 UI_hvacReqUserPowerState 0 ""OFF"" 1 ""ON"" 4 ""OVERHEAT_PROTECT"" 3 ""OVERHEAT_PROTECT_FANONLY"" 2 ""PRECONDITION"" ;
VAL_ 787 UI_trackCmpOverclock 0 ""TRACK_MODE_CMP_OVERCLOCK_OFF"" 1 ""TRACK_MODE_CMP_OVERCLOCK_ON"" ;
VAL_ 787 UI_trackModeRequest 0 ""TRACK_MODE_REQUEST_IDLE"" 2 ""TRACK_MODE_REQUEST_OFF"" 1 ""TRACK_MODE_REQUEST_ON"" ;
VAL_ 787 UI_trackPostCooling 0 ""TRACK_MODE_POST_COOLING_OFF"" 1 ""TRACK_MODE_POST_COOLING_ON"" ;
VAL_ 821 DIR_infoIndex 13 ""DI_INFO_APP_CRC"" 17 ""DI_INFO_APP_GITHASH"" 15 ""DI_INFO_BOOTLOADER_CRC"" 18 ""DI_INFO_BOOTLOADER_GITHASH"" 14 ""DI_INFO_BOOTLOADER_SVN"" 10 ""DI_INFO_BUILD_HWID_COMPONENTID"" 0 ""DI_INFO_DEPRECATED_0"" 1 ""DI_INFO_DEPRECATED_1"" 2 ""DI_INFO_DEPRECATED_2"" 3 ""DI_INFO_DEPRECATED_3"" 4 ""DI_INFO_DEPRECATED_4"" 5 ""DI_INFO_DEPRECATED_5"" 6 ""DI_INFO_DEPRECATED_6"" 7 ""DI_INFO_DEPRECATED_7"" 8 ""DI_INFO_DEPRECATED_8"" 9 ""DI_INFO_DEPRECATED_9"" 255 ""DI_INFO_END"" 11 ""DI_INFO_PCBAID_ASSYID_USAGEID"" 16 ""DI_INFO_SUBCOMPONENT"" 23 ""DI_INFO_SUBCOMPONENT2"" 31 ""DI_INFO_SUBCOMPONENT_GITHASH"" 20 ""DI_INFO_UDS_PROTOCOL_BOOTCRC"" 19 ""DI_INFO_VERSION_DEPRECATED"" ;
VAL_ 821 DIR_FPGA_version 254 ""FPGA_VERSION_LOCAL_BUILD"" 255 ""FPGA_VERSION_SNA"" ;
VAL_ 821 DIR_buildType 2 ""INFO_LOCAL_BUILD"" 4 ""INFO_MFG_BUILD"" 1 ""INFO_PLATFORM_BUILD"" 3 ""INFO_TRACEABLE_CI_BUILD"" 0 ""INFO_UNKNOWN_BUILD"" ;
VAL_ 821 DIR_hardwareId 252 ""CONFIGURABLE_HWID_PLACEHOLDER"" ;
VAL_ 821 DIR_oilPumpBuildType 2 ""INFO_LOCAL_BUILD"" 4 ""INFO_MFG_BUILD"" 1 ""INFO_PLATFORM_BUILD"" 3 ""INFO_TRACEABLE_CI_BUILD"" 0 ""INFO_UNKNOWN_BUILD"" ;
VAL_ 899 VCRIGHT_estimatedThsSolarLoad 1023 ""SNA"" ;
VAL_ 899 VCRIGHT_estimatedVehicleSituatio 1 ""VEHICLE_SITUATION_INDOOR"" 2 ""VEHICLE_SITUATION_OUTDOOR"" 0 ""VEHICLE_SITUATION_UNKNOWN"" ;
VAL_ 899 VCRIGHT_thsHumidity 255 ""SNA"" ;
VAL_ 899 VCRIGHT_thsSolarLoadInfrared 1023 ""SNA"" ;
VAL_ 899 VCRIGHT_thsSolarLoadVisible 1023 ""SNA"" ;
VAL_ 899 VCRIGHT_thsTemperature 128 ""SNA"" ;
VAL_ 947 UI_autopilotPowerStateRequest 0 ""AUTOPILOT_NOMINAL"" 1 ""AUTOPILOT_SENTRY"" 2 ""AUTOPILOT_SUSPEND"" ;
VAL_ 947 UI_lightSwitch 0 ""LIGHT_SWITCH_AUTO"" 3 ""LIGHT_SWITCH_OFF"" 1 ""LIGHT_SWITCH_ON"" 2 ""LIGHT_SWITCH_PARKING"" 4 ""LIGHT_SWITCH_SNA"" ;
VAL_ 947 UI_shorted12VCellTestMode 2 ""SHORTED_CELL_TEST_MODE_ACTIVE"" 0 ""SHORTED_CELL_TEST_MODE_DISABLED"" 1 ""SHORTED_CELL_TEST_MODE_SHADOW"" ;
VAL_ 947 UI_summonState 3 ""ACTIVE"" 1 ""IDLE"" 2 ""PRE_PRIMED"" 0 ""SNA"" ;
VAL_ 947 UI_windowRequest 3 ""WINDOW_REQUEST_GOTO_CLOSED"" 4 ""WINDOW_REQUEST_GOTO_OPEN"" 1 ""WINDOW_REQUEST_GOTO_PERCENT"" 2 ""WINDOW_REQUEST_GOTO_VENT"" 0 ""WINDOW_REQUEST_IDLE"" ;
VAL_ 963 VCRIGHT_frontBuckleSwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontOccupancySwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatBackrestBack 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatBackrestForward 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatLiftDown 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatLiftUp 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatLumbarDown 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatLumbarIn 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatLumbarOut 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatLumbarUp 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatTiltDown 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatTiltUp 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatTrackBack 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_frontSeatTrackForward 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_rearCenterBuckleSwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 963 VCRIGHT_rearRightBuckleSwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 995 VCRIGHT_brakeLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 995 VCRIGHT_interiorTrunkLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 995 VCRIGHT_rearFogLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 995 VCRIGHT_reverseLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 995 VCRIGHT_tailLightStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 995 VCRIGHT_turnSignalStatus 2 ""LIGHT_FAULT"" 0 ""LIGHT_OFF"" 1 ""LIGHT_ON"" 3 ""LIGHT_SNA"" ;
VAL_ 1622 DIF_infoIndex 13 ""DI_INFO_APP_CRC"" 17 ""DI_INFO_APP_GITHASH"" 15 ""DI_INFO_BOOTLOADER_CRC"" 18 ""DI_INFO_BOOTLOADER_GITHASH"" 14 ""DI_INFO_BOOTLOADER_SVN"" 10 ""DI_INFO_BUILD_HWID_COMPONENTID"" 0 ""DI_INFO_DEPRECATED_0"" 1 ""DI_INFO_DEPRECATED_1"" 2 ""DI_INFO_DEPRECATED_2"" 3 ""DI_INFO_DEPRECATED_3"" 4 ""DI_INFO_DEPRECATED_4"" 5 ""DI_INFO_DEPRECATED_5"" 6 ""DI_INFO_DEPRECATED_6"" 7 ""DI_INFO_DEPRECATED_7"" 8 ""DI_INFO_DEPRECATED_8"" 9 ""DI_INFO_DEPRECATED_9"" 255 ""DI_INFO_END"" 11 ""DI_INFO_PCBAID_ASSYID_USAGEID"" 16 ""DI_INFO_SUBCOMPONENT"" 23 ""DI_INFO_SUBCOMPONENT2"" 31 ""DI_INFO_SUBCOMPONENT_GITHASH"" 20 ""DI_INFO_UDS_PROTOCOL_BOOTCRC"" 19 ""DI_INFO_VERSION_DEPRECATED"" ;
VAL_ 1622 DIF_FPGA_version 254 ""FPGA_VERSION_LOCAL_BUILD"" 255 ""FPGA_VERSION_SNA"" ;
VAL_ 1622 DIF_buildType 2 ""INFO_LOCAL_BUILD"" 4 ""INFO_MFG_BUILD"" 1 ""INFO_PLATFORM_BUILD"" 3 ""INFO_TRACEABLE_CI_BUILD"" 0 ""INFO_UNKNOWN_BUILD"" ;
VAL_ 1622 DIF_hardwareId 252 ""CONFIGURABLE_HWID_PLACEHOLDER"" ;
VAL_ 1622 DIF_oilPumpBuildType 2 ""INFO_LOCAL_BUILD"" 4 ""INFO_MFG_BUILD"" 1 ""INFO_PLATFORM_BUILD"" 3 ""INFO_TRACEABLE_CI_BUILD"" 0 ""INFO_UNKNOWN_BUILD"" ;
VAL_ 768 BMS_infoIndex 13 ""INFO_APP_CRC"" 17 ""INFO_APP_GITHASH"" 15 ""INFO_BOOTLOADER_CRC"" 18 ""INFO_BOOTLOADER_GITHASH"" 14 ""INFO_BOOTLOADER_SVN"" 10 ""INFO_BUILD_HWID_COMPONENTID"" 0 ""INFO_DEPRECATED_0"" 1 ""INFO_DEPRECATED_1"" 2 ""INFO_DEPRECATED_2"" 3 ""INFO_DEPRECATED_3"" 4 ""INFO_DEPRECATED_4"" 5 ""INFO_DEPRECATED_5"" 6 ""INFO_DEPRECATED_6"" 7 ""INFO_DEPRECATED_7"" 8 ""INFO_DEPRECATED_8"" 9 ""INFO_DEPRECATED_9"" 255 ""INFO_END"" 27 ""INFO_PACKAGE_PN_15_20"" 25 ""INFO_PACKAGE_PN_1_7"" 26 ""INFO_PACKAGE_PN_8_14"" 29 ""INFO_PACKAGE_SN_1_7"" 30 ""INFO_PACKAGE_SN_8_14"" 11 ""INFO_PCBAID_ASSYID_USAGEID"" 16 ""INFO_SUBCOMPONENT"" 31 ""INFO_SUBCOMPONENT_GITHASH"" 20 ""INFO_UDS_PROTOCOL_BOOTCRC"" 22 ""INFO_VARIANTCRC"" 19 ""INFO_VERSION_DEPRECATED"" ;
VAL_ 768 BMS_buildType 2 ""INFO_LOCAL_BUILD"" 4 ""INFO_MFG_BUILD"" 1 ""INFO_PLATFORM_BUILD"" 3 ""INFO_TRACEABLE_CI_BUILD"" 0 ""INFO_UNKNOWN_BUILD"" ;
VAL_ 530 BMS_chgPowerAvailable 2047 ""SNA"" ;
VAL_ 530 BMS_contactorState 6 ""BMS_CTRSET_BLOCKED"" 4 ""BMS_CTRSET_CLOSED"" 3 ""BMS_CTRSET_CLOSING"" 1 ""BMS_CTRSET_OPEN"" 2 ""BMS_CTRSET_OPENING"" 0 ""BMS_CTRSET_SNA"" 5 ""BMS_CTRSET_WELDED"" ;
VAL_ 530 BMS_diLimpRequest 0 ""LIMP_REQUEST_NONE"" 1 ""LIMP_REQUEST_WELDED"" ;
VAL_ 530 BMS_ecuLogUploadRequest 1 ""REQUEST_PRIORITY_1"" 2 ""REQUEST_PRIORITY_2"" 3 ""REQUEST_PRIORITY_3"" 0 ""REQUEST_PRIORITY_NONE"" ;
VAL_ 530 BMS_hvState 1 ""HV_COMING_UP"" 0 ""HV_DOWN"" 2 ""HV_GOING_DOWN"" 6 ""HV_UP"" 4 ""HV_UP_FOR_CHARGE"" 5 ""HV_UP_FOR_DC_CHARGE"" 3 ""HV_UP_FOR_DRIVE"" ;
VAL_ 530 BMS_isolationResistance 1023 ""SNA"" ;
VAL_ 530 BMS_smStateRequest 3 ""BMS_CHARGE"" 5 ""BMS_CLEAR_FAULT"" 10 ""BMS_DIAG"" 1 ""BMS_DRIVE"" 6 ""BMS_FAULT"" 4 ""BMS_FEIM"" 9 ""BMS_SNA"" 0 ""BMS_STANDBY"" 2 ""BMS_SUPPORT"" 8 ""BMS_TEST"" 7 ""BMS_WELD"" ;
VAL_ 530 BMS_state 3 ""BMS_CHARGE"" 5 ""BMS_CLEAR_FAULT"" 10 ""BMS_DIAG"" 1 ""BMS_DRIVE"" 6 ""BMS_FAULT"" 4 ""BMS_FEIM"" 9 ""BMS_SNA"" 0 ""BMS_STANDBY"" 2 ""BMS_SUPPORT"" 8 ""BMS_TEST"" 7 ""BMS_WELD"" ;
VAL_ 530 BMS_uiChargeStatus 2 ""BMS_ABOUT_TO_CHARGE"" 4 ""BMS_CHARGE_COMPLETE"" 5 ""BMS_CHARGE_STOPPED"" 3 ""BMS_CHARGING"" 0 ""BMS_DISCONNECTED"" 1 ""BMS_NO_POWER"" ;
VAL_ 796 CC_currentLimit 255 ""SNA"" ;
VAL_ 796 CC_gridGrounding 1 ""CC_GRID_GROUNDING_IT_SplitPhase"" 2 ""CC_GRID_GROUNDING_SNA"" 0 ""CC_GRID_GROUNDING_TN_TT"" ;
VAL_ 796 CC_line1Voltage 511 ""SNA"" ;
VAL_ 796 CC_line2Voltage 511 ""SNA"" ;
VAL_ 796 CC_line3Voltage 511 ""SNA"" ;
VAL_ 796 CC_numPhases 0 ""SNA"" ;
VAL_ 796 CC_pilotState 2 ""CC_PILOT_STATE_FAULTED"" 1 ""CC_PILOT_STATE_IDLE"" 0 ""CC_PILOT_STATE_READY"" 3 ""CC_PILOT_STATE_SNA"" ;
VAL_ 573 CP_chargeShutdownRequest 2 ""EMERGENCY_SHUTDOWN_REQUESTED"" 1 ""GRACEFUL_SHUTDOWN_REQUESTED"" 0 ""NO_SHUTDOWN_REQUESTED"" ;
VAL_ 573 CP_hvChargeStatus 1 ""CP_CHARGE_CONNECTED"" 5 ""CP_CHARGE_ENABLED"" 6 ""CP_CHARGE_FAULTED"" 0 ""CP_CHARGE_INACTIVE"" 2 ""CP_CHARGE_STANDBY"" 4 ""CP_EVSE_TEST_PASSED"" 3 ""CP_EXT_EVSE_TEST_ACTIVE"" ;
VAL_ 317 CP_chargeShutdownRequest 2 ""EMERGENCY_SHUTDOWN_REQUESTED"" 1 ""GRACEFUL_SHUTDOWN_REQUESTED"" 0 ""NO_SHUTDOWN_REQUESTED"" ;
VAL_ 317 CP_evseChargeType 2 ""AC_CHARGER_PRESENT"" 1 ""DC_CHARGER_PRESENT"" 0 ""NO_CHARGER_PRESENT"" ;
VAL_ 317 CP_hvChargeStatus 1 ""CP_CHARGE_CONNECTED"" 5 ""CP_CHARGE_ENABLED"" 6 ""CP_CHARGE_FAULTED"" 0 ""CP_CHARGE_INACTIVE"" 2 ""CP_CHARGE_STANDBY"" 4 ""CP_EVSE_TEST_PASSED"" 3 ""CP_EXT_EVSE_TEST_ACTIVE"" ;
VAL_ 1085 CP_chargeShutdownRequest_log 2 ""EMERGENCY_SHUTDOWN_REQUESTED"" 1 ""GRACEFUL_SHUTDOWN_REQUESTED"" 0 ""NO_SHUTDOWN_REQUESTED"" ;
VAL_ 1085 CP_evseChargeType_log 2 ""AC_CHARGER_PRESENT"" 1 ""DC_CHARGER_PRESENT"" 0 ""NO_CHARGER_PRESENT"" ;
VAL_ 1085 CP_hvChargeStatus_log 1 ""CP_CHARGE_CONNECTED"" 5 ""CP_CHARGE_ENABLED"" 6 ""CP_CHARGE_FAULTED"" 0 ""CP_CHARGE_INACTIVE"" 2 ""CP_CHARGE_STANDBY"" 4 ""CP_EVSE_TEST_PASSED"" 3 ""CP_EXT_EVSE_TEST_ACTIVE"" ;
VAL_ 541 CP_acChargeState 1 ""AC_CHARGE_CONNECTED_CHARGE_BLOCKED"" 3 ""AC_CHARGE_ENABLED"" 6 ""AC_CHARGE_FAULT"" 0 ""AC_CHARGE_INACTIVE"" 4 ""AC_CHARGE_ONBOARD_CHARGER_SHUTDOWN"" 2 ""AC_CHARGE_STANDBY"" 5 ""AC_CHARGE_VEH_SHUTDOWN"" ;
VAL_ 541 CP_cableType 2 ""CHG_CABLE_TYPE_GB_AC"" 3 ""CHG_CABLE_TYPE_GB_DC"" 0 ""CHG_CABLE_TYPE_IEC"" 1 ""CHG_CABLE_TYPE_SAE"" 4 ""CHG_CABLE_TYPE_SNA"" ;
VAL_ 541 CP_evseChargeType_UI 2 ""AC_CHARGER_PRESENT"" 1 ""DC_CHARGER_PRESENT"" 0 ""NO_CHARGER_PRESENT"" ;
VAL_ 541 CP_gbState 10 ""GBDC_CHARGE_DISABLING"" 5 ""GBDC_CHARGE_PARAM_CONFIG"" 8 ""GBDC_CHARGING"" 2 ""GBDC_COMMS_RECEIVED"" 11 ""GBDC_END_OF_CHARGE"" 12 ""GBDC_ERROR_HANDLING"" 14 ""GBDC_FAULTED"" 3 ""GBDC_HANDSHAKING_EXT_ISO"" 0 ""GBDC_INACTIVE"" 7 ""GBDC_READY_TO_CHARGE"" 4 ""GBDC_RECOGNITION"" 13 ""GBDC_RETRY_CHARGE"" 9 ""GBDC_STOP_CHARGE_REQUESTED"" 15 ""GBDC_TESTER_PRESENT"" 6 ""GBDC_VEH_PACK_PRECHARGE"" 1 ""GBDC_WAIT_FOR_COMMS"" ;
VAL_ 541 CP_gbdcFailureReason 1 ""GBDC_ATTEMPTS_EXPIRED"" 3 ""GBDC_CRITICAL_FAILURE"" 0 ""GBDC_FAILURE_NONE"" 2 ""GBDC_SHUTDOWN_FAILURE"" ;
VAL_ 541 CP_gbdcStopChargeReason 3 ""GBDC_COMMS_TIMEOUT"" 5 ""GBDC_EVSE_CRITICAL_FAULT"" 4 ""GBDC_EVSE_FAULT"" 2 ""GBDC_EVSE_REQUESTED"" 6 ""GBDC_LIVE_DISCONNECT"" 0 ""GBDC_STOP_REASON_NONE"" 7 ""GBDC_SUPERCHARGER_COMMS_TIMEOUT"" 1 ""GBDC_VEH_REQUESTED"" ;
VAL_ 541 CP_iecComboState 6 ""IEC_COMBO_CABLE_CHECK"" 5 ""IEC_COMBO_CHARGE_PARAM_DISCOVERY"" 1 ""IEC_COMBO_CONNECTED"" 8 ""IEC_COMBO_ENABLED"" 10 ""IEC_COMBO_END_OF_CHARGE"" 11 ""IEC_COMBO_FAULT"" 0 ""IEC_COMBO_INACTIVE"" 4 ""IEC_COMBO_PAYMENT_SELECTION"" 7 ""IEC_COMBO_PRECHARGE"" 3 ""IEC_COMBO_SERVICE_DISCOVERY"" 9 ""IEC_COMBO_SHUTDOWN"" 2 ""IEC_COMBO_V2G_SESSION_SETUP"" 12 ""IEC_COMBO_WAIT_RESTART"" ;
VAL_ 541 CP_pilot 3 ""CHG_PILOT_FAST_CHARGE"" 1 ""CHG_PILOT_FAULTED"" 4 ""CHG_PILOT_IDLE"" 5 ""CHG_PILOT_INVALID"" 2 ""CHG_PILOT_LINE_CHARGE"" 0 ""CHG_PILOT_NONE"" 7 ""CHG_PILOT_SNA"" 6 ""CHG_PILOT_UNUSED_6"" ;
VAL_ 541 CP_proximity 1 ""CHG_PROXIMITY_DISCONNECTED"" 3 ""CHG_PROXIMITY_LATCHED"" 0 ""CHG_PROXIMITY_SNA"" 2 ""CHG_PROXIMITY_UNLATCHED"" ;
VAL_ 541 CP_teslaDcState 1 ""TESLA_DC_CONNECTED_CHARGE_BLOCKED"" 9 ""TESLA_DC_EMERGENCY_SHUTDOWN"" 6 ""TESLA_DC_ENABLED"" 7 ""TESLA_DC_EVSE_SHUTDOWN"" 5 ""TESLA_DC_EXT_PRECHARGE_ACTIVE"" 3 ""TESLA_DC_EXT_TESTS_ENABLED"" 4 ""TESLA_DC_EXT_TEST_ACTIVE"" 10 ""TESLA_DC_FAULT"" 0 ""TESLA_DC_INACTIVE"" 2 ""TESLA_DC_STANDBY"" 8 ""TESLA_DC_VEH_SHUTDOWN"" ;
VAL_ 541 CP_teslaSwcanState 1 ""TESLA_SWCAN_ACCEPT"" 3 ""TESLA_SWCAN_ESTABLISHED"" 4 ""TESLA_SWCAN_FAULT"" 5 ""TESLA_SWCAN_GO_TO_SLEEP"" 0 ""TESLA_SWCAN_INACTIVE"" 6 ""TESLA_SWCAN_OFFBOARD_UPDATE_IN_PROGRESS"" 2 ""TESLA_SWCAN_RECEIVE"" ;
VAL_ 1859 VCRIGHT_mirrorRecallStatus 2 ""RECALL_COMPLETE"" 3 ""RECALL_INTERRUPTED"" 1 ""RECALL_IN_PROGRESS"" 0 ""RECALL_SNA"" ;
VAL_ 1859 VCRIGHT_seatRecallStatus 2 ""RECALL_COMPLETE"" 3 ""RECALL_INTERRUPTED"" 1 ""RECALL_IN_PROGRESS"" 0 ""RECALL_SNA"" ;
VAL_ 1859 VCRIGHT_systemRecallStatus 2 ""RECALL_COMPLETE"" 3 ""RECALL_INTERRUPTED"" 1 ""RECALL_IN_PROGRESS"" 0 ""RECALL_SNA"" ;
VAL_ 1885 CP_sensorDataSelect 2 ""CP_SENSOR_DOOR"" 0 ""CP_SENSOR_DOOR_COUNTS"" 4 ""CP_SENSOR_INDUCTIVE_DOOR"" 12 ""CP_SENSOR_INLET_HARNESS_ID"" 3 ""CP_SENSOR_LATCH"" 9 ""CP_SENSOR_PILOT"" 10 ""CP_SENSOR_PILOT2"" 1 ""CP_SENSOR_PIN_TEMP"" 8 ""CP_SENSOR_PROX"" 11 ""CP_SENSOR_PROX_GB"" 7 ""CP_SENSOR_RAILS"" 5 ""CP_SENSOR_SAFETY"" 6 ""CP_SENSOR_UHF"" ;
VAL_ 1885 CP_inlet1HarnessIdState 1 ""HARNESS_PEDIGREE_INVALID"" 0 ""HARNESS_PEDIGREE_UNKNOWN_SNA"" 2 ""HARNESS_PEDIGREE_VALID"" ;
VAL_ 1885 CP_inlet2HarnessIdState 1 ""HARNESS_PEDIGREE_INVALID"" 0 ""HARNESS_PEDIGREE_UNKNOWN_SNA"" 2 ""HARNESS_PEDIGREE_VALID"" ;
VAL_ 819 UI_acChargeCurrentLimit 127 ""SNA"" ;
VAL_ 819 UI_brickBalancingDisabled 0 ""FALSE"" 1 ""TRUE"" ;
VAL_ 819 UI_brickVLoggingRequest 0 ""FALSE"" 1 ""TRUE"" ;
VAL_ 820 UI_limitMode 2 ""LIMIT_FACTORY"" 0 ""LIMIT_NORMAL"" 3 ""LIMIT_SERVICE"" 1 ""LIMIT_VALET"" ;
VAL_ 820 UI_motorOnMode 1 ""MOTORONMODE_FRONT_ONLY"" 0 ""MOTORONMODE_NORMAL"" 2 ""MOTORONMODE_REAR_ONLY"" ;
VAL_ 820 UI_pedalMap 0 ""CHILL"" 2 ""PERFORMANCE"" 1 ""SPORT"" ;
VAL_ 820 UI_speedLimit 255 ""SNA"" ;
VAL_ 820 UI_stoppingMode 1 ""CREEP"" 2 ""HOLD"" 0 ""STANDARD"" ;
VAL_ 820 UI_systemPowerLimit 31 ""SNA"" ;
VAL_ 820 UI_systemTorqueLimit 63 ""SNA"" ;
VAL_ 820 UI_wasteMode 2 ""WASTE_TYPE_FULL"" 0 ""WASTE_TYPE_NONE"" 1 ""WASTE_TYPE_PARTIAL"" ;
VAL_ 820 UI_wasteModeRegenLimit 3 ""MAX_REGEN_CURRENT_0A"" 2 ""MAX_REGEN_CURRENT_10A"" 1 ""MAX_REGEN_CURRENT_30A"" 0 ""MAX_REGEN_CURRENT_MAX"" ;
VAL_ 820 UI_closureConfirmed 1 ""CONFIRMED_FRUNK"" 0 ""CONFIRMED_NONE"" 2 ""CONFIRMED_PROX"" ;
VAL_ 577 VCFRONT_coolantAirPurgeBatState 1 ""AIR_PURGE_STATE_ACTIVE"" 2 ""AIR_PURGE_STATE_COMPLETE"" 0 ""AIR_PURGE_STATE_INACTIVE"" 3 ""AIR_PURGE_STATE_INTERRUPTED"" 4 ""AIR_PURGE_STATE_PENDING"" ;
VAL_ 577 VCFRONT_coolantFlowBatReason 4 ""ACTIVE_MANAGER_BATT"" 9 ""ACTIVE_MANAGER_PT"" 6 ""BMS_FLOW_REQ"" 1 ""COOLANT_AIR_PURGE"" 7 ""DAS_FLOW_REQ"" 13 ""DIS_FLOW_REQ"" 12 ""DI_FLOW_REQ"" 14 ""HP_FLOW_REQ"" 0 ""NONE"" 2 ""NO_FLOW_REQ"" 3 ""OVERRIDE_BATT"" 8 ""OVERRIDE_PT"" 5 ""PASSIVE_MANAGER_BATT"" 10 ""PASSIVE_MANAGER_PT"" 11 ""PCS_FLOW_REQ"" ;
VAL_ 577 VCFRONT_coolantFlowPTReason 4 ""ACTIVE_MANAGER_BATT"" 9 ""ACTIVE_MANAGER_PT"" 6 ""BMS_FLOW_REQ"" 1 ""COOLANT_AIR_PURGE"" 7 ""DAS_FLOW_REQ"" 13 ""DIS_FLOW_REQ"" 12 ""DI_FLOW_REQ"" 14 ""HP_FLOW_REQ"" 0 ""NONE"" 2 ""NO_FLOW_REQ"" 3 ""OVERRIDE_BATT"" 8 ""OVERRIDE_PT"" 5 ""PASSIVE_MANAGER_BATT"" 10 ""PASSIVE_MANAGER_PT"" 11 ""PCS_FLOW_REQ"" ;
VAL_ 577 VCFRONT_wasteHeatRequestType 2 ""WASTE_TYPE_FULL"" 0 ""WASTE_TYPE_NONE"" 1 ""WASTE_TYPE_PARTIAL"" ;
VAL_ 1493 DI_IGBTJunctTemp 255 ""SNA"" ;
VAL_ 1366 DIF_IGBTJunctTemp 255 ""SNA"" ;
VAL_ 2005 DIR_fluxState 5 ""DI_FLUXSTATE_ENABLED"" 9 ""DI_FLUXSTATE_FAULT"" 4 ""DI_FLUXSTATE_FLUX_DOWN"" 3 ""DI_FLUXSTATE_FLUX_UP"" 6 ""DI_FLUXSTATE_ICONTROL"" 2 ""DI_FLUXSTATE_STANDBY"" 0 ""DI_FLUXSTATE_START"" 10 ""DI_FLUXSTATE_STATIONARY_WASTE"" 1 ""DI_FLUXSTATE_TEST"" 7 ""DI_FLUXSTATE_VCONTROL"" ;
VAL_ 2005 DIR_gateDriveState 3 ""PSTG_GD_STATE_CONFIGURED"" 2 ""PSTG_GD_STATE_CONFIGURING"" 0 ""PSTG_GD_STATE_INIT"" 4 ""PSTG_GD_STATE_NOT_CONFIGURED"" 1 ""PSTG_GD_STATE_SELFTEST"" ;
VAL_ 2005 DIR_gateDriveSupplyState 0 ""PSTG_GD_SUPPLY_DOWN"" 3 ""PSTG_GD_SUPPLY_FALLING"" 1 ""PSTG_GD_SUPPLY_RISING"" 2 ""PSTG_GD_SUPPLY_UP"" ;
VAL_ 2005 DIR_magnetTempEst 0 ""SNA"" ;
VAL_ 2005 DIR_motorType 8 ""DI_MOTOR_F1A"" 10 ""DI_MOTOR_F1AC"" 11 ""DI_MOTOR_F2AB"" 12 ""DI_MOTOR_F2AC"" 13 ""DI_MOTOR_F2AD"" 14 ""DI_MOTOR_F2AE"" 26 ""DI_MOTOR_F2AE_AL"" 15 ""DI_MOTOR_F2APMSRM"" 17 ""DI_MOTOR_IM100A"" 19 ""DI_MOTOR_IM100B"" 22 ""DI_MOTOR_IM130C"" 24 ""DI_MOTOR_IM130D"" 25 ""DI_MOTOR_IM130D_AL"" 27 ""DI_MOTOR_IM130D_AL_POSCO"" 20 ""DI_MOTOR_IM216A"" 3 ""DI_MOTOR_M7M3"" 4 ""DI_MOTOR_M7M4"" 5 ""DI_MOTOR_M7M5"" 7 ""DI_MOTOR_M7M6"" 6 ""DI_MOTOR_M8A"" 16 ""DI_MOTOR_PM216A"" 18 ""DI_MOTOR_PM216B"" 21 ""DI_MOTOR_PM216C"" 30 ""DI_MOTOR_PM216CSR"" 31 ""DI_MOTOR_PM216CSR_N42"" 23 ""DI_MOTOR_PM216D"" 32 ""DI_MOTOR_PM228B"" 28 ""DI_MOTOR_PM275B"" 29 ""DI_MOTOR_PM350B"" 1 ""DI_MOTOR_ROADSTER_BASE"" 2 ""DI_MOTOR_ROADSTER_SPORT"" 0 ""DI_MOTOR_SNA"" 9 ""DI_MOTOR_SSR1A"" ;
VAL_ 2005 DIR_oilPumpPhaseVoltage 255 ""SNA"" ;
VAL_ 2005 DIR_oilPumpPressureEstimateMax 255 ""SNA"" ;
VAL_ 2005 DIR_oilPumpPressureExpectedMin 255 ""SNA"" ;
VAL_ 2005 DIR_powerStageSafeState 2 ""PSTG_SAFESTATE_3PS_HIGH"" 3 ""PSTG_SAFESTATE_3PS_LOW"" 1 ""PSTG_SAFESTATE_ALL_OFF"" 0 ""PSTG_SAFESTATE_NONE"" ;
VAL_ 2005 DIR_pwmState 1 ""PWMSTATE_DPWM2"" 2 ""PWMSTATE_OPWM1"" 3 ""PWMSTATE_OPWM2"" 0 ""PWMSTATE_SVPWM"" ;
VAL_ 2005 DIR_rotorOffsetLearningState 9 ""ROL_NUM_STATES"" 2 ""ROL_STATE_ACCELERATE"" 5 ""ROL_STATE_CORRECT"" 8 ""ROL_STATE_DONE"" 0 ""ROL_STATE_INIT"" 4 ""ROL_STATE_MEASURE"" 3 ""ROL_STATE_SHIFT"" 6 ""ROL_STATE_VERIFY"" 1 ""ROL_STATE_WAIT"" 7 ""ROL_STATE_WRITE"" ;
VAL_ 2005 DIR_ssmState 5 ""SSM_STATE_ABORT"" 4 ""SSM_STATE_ENABLE"" 8 ""SSM_STATE_FAULT"" 2 ""SSM_STATE_IDLE"" 7 ""SSM_STATE_RETRY"" 3 ""SSM_STATE_STANDBY"" 0 ""SSM_STATE_START"" 1 ""SSM_STATE_UNAVAILABLE"" 6 ""SSM_STATE_WAIT_FOR_RETRY"" ;
VAL_ 2005 DIR_tcMaxRequest 255 ""SNA"" ;
VAL_ 2005 DIR_tcMinRequest 255 ""SNA"" ;
VAL_ 2005 DIR_usmState 3 ""USM_STATE_ABORT"" 4 ""USM_STATE_ENABLE"" 5 ""USM_STATE_FAULT"" 2 ""USM_STATE_RETRY"" 1 ""USM_STATE_STANDBY"" 0 ""USM_STATE_START"" 6 ""USM_STATE_UNAVAILABLE"" 7 ""USM_STATE_WAIT_FOR_RETRY"" ;
VAL_ 1879 DIF_fluxState 5 ""DI_FLUXSTATE_ENABLED"" 9 ""DI_FLUXSTATE_FAULT"" 4 ""DI_FLUXSTATE_FLUX_DOWN"" 3 ""DI_FLUXSTATE_FLUX_UP"" 6 ""DI_FLUXSTATE_ICONTROL"" 2 ""DI_FLUXSTATE_STANDBY"" 0 ""DI_FLUXSTATE_START"" 10 ""DI_FLUXSTATE_STATIONARY_WASTE"" 1 ""DI_FLUXSTATE_TEST"" 7 ""DI_FLUXSTATE_VCONTROL"" ;
VAL_ 1879 DIF_gateDriveState 3 ""PSTG_GD_STATE_CONFIGURED"" 2 ""PSTG_GD_STATE_CONFIGURING"" 0 ""PSTG_GD_STATE_INIT"" 4 ""PSTG_GD_STATE_NOT_CONFIGURED"" 1 ""PSTG_GD_STATE_SELFTEST"" ;
VAL_ 1879 DIF_gateDriveSupplyState 0 ""PSTG_GD_SUPPLY_DOWN"" 3 ""PSTG_GD_SUPPLY_FALLING"" 1 ""PSTG_GD_SUPPLY_RISING"" 2 ""PSTG_GD_SUPPLY_UP"" ;
VAL_ 1879 DIF_magnetTempEst 0 ""SNA"" ;
VAL_ 1879 DIF_motorType 8 ""DI_MOTOR_F1A"" 10 ""DI_MOTOR_F1AC"" 11 ""DI_MOTOR_F2AB"" 12 ""DI_MOTOR_F2AC"" 13 ""DI_MOTOR_F2AD"" 14 ""DI_MOTOR_F2AE"" 26 ""DI_MOTOR_F2AE_AL"" 15 ""DI_MOTOR_F2APMSRM"" 17 ""DI_MOTOR_IM100A"" 19 ""DI_MOTOR_IM100B"" 22 ""DI_MOTOR_IM130C"" 24 ""DI_MOTOR_IM130D"" 25 ""DI_MOTOR_IM130D_AL"" 27 ""DI_MOTOR_IM130D_AL_POSCO"" 20 ""DI_MOTOR_IM216A"" 3 ""DI_MOTOR_M7M3"" 4 ""DI_MOTOR_M7M4"" 5 ""DI_MOTOR_M7M5"" 7 ""DI_MOTOR_M7M6"" 6 ""DI_MOTOR_M8A"" 16 ""DI_MOTOR_PM216A"" 18 ""DI_MOTOR_PM216B"" 21 ""DI_MOTOR_PM216C"" 30 ""DI_MOTOR_PM216CSR"" 31 ""DI_MOTOR_PM216CSR_N42"" 23 ""DI_MOTOR_PM216D"" 32 ""DI_MOTOR_PM228B"" 28 ""DI_MOTOR_PM275B"" 29 ""DI_MOTOR_PM350B"" 1 ""DI_MOTOR_ROADSTER_BASE"" 2 ""DI_MOTOR_ROADSTER_SPORT"" 0 ""DI_MOTOR_SNA"" 9 ""DI_MOTOR_SSR1A"" ;
VAL_ 1879 DIF_oilPumpPhaseVoltage 255 ""SNA"" ;
VAL_ 1879 DIF_oilPumpPressureEstimateMax 255 ""SNA"" ;
VAL_ 1879 DIF_oilPumpPressureExpectedMin 255 ""SNA"" ;
VAL_ 1879 DIF_powerStageSafeState 2 ""PSTG_SAFESTATE_3PS_HIGH"" 3 ""PSTG_SAFESTATE_3PS_LOW"" 1 ""PSTG_SAFESTATE_ALL_OFF"" 0 ""PSTG_SAFESTATE_NONE"" ;
VAL_ 1879 DIF_pwmState 1 ""PWMSTATE_DPWM2"" 2 ""PWMSTATE_OPWM1"" 3 ""PWMSTATE_OPWM2"" 0 ""PWMSTATE_SVPWM"" ;
VAL_ 1879 DIF_rotorOffsetLearningState 9 ""ROL_NUM_STATES"" 2 ""ROL_STATE_ACCELERATE"" 5 ""ROL_STATE_CORRECT"" 8 ""ROL_STATE_DONE"" 0 ""ROL_STATE_INIT"" 4 ""ROL_STATE_MEASURE"" 3 ""ROL_STATE_SHIFT"" 6 ""ROL_STATE_VERIFY"" 1 ""ROL_STATE_WAIT"" 7 ""ROL_STATE_WRITE"" ;
VAL_ 1879 DIF_ssmState 5 ""SSM_STATE_ABORT"" 4 ""SSM_STATE_ENABLE"" 8 ""SSM_STATE_FAULT"" 2 ""SSM_STATE_IDLE"" 7 ""SSM_STATE_RETRY"" 3 ""SSM_STATE_STANDBY"" 0 ""SSM_STATE_START"" 1 ""SSM_STATE_UNAVAILABLE"" 6 ""SSM_STATE_WAIT_FOR_RETRY"" ;
VAL_ 1879 DIF_tcMaxRequest 255 ""SNA"" ;
VAL_ 1879 DIF_tcMinRequest 255 ""SNA"" ;
VAL_ 1879 DIF_usmState 3 ""USM_STATE_ABORT"" 4 ""USM_STATE_ENABLE"" 5 ""USM_STATE_FAULT"" 2 ""USM_STATE_RETRY"" 1 ""USM_STATE_STANDBY"" 0 ""USM_STATE_START"" 6 ""USM_STATE_UNAVAILABLE"" 7 ""USM_STATE_WAIT_FOR_RETRY"" ;
VAL_ 694 DI_ptcStateUI 0 ""FAULTED"" 1 ""BACKUP"" 2 ""ON"" 3 ""SNA"" ;
VAL_ 694 DI_tractionControlModeUI 0 ""NORMAL"" 1 ""SLIP_START"" 2 ""DEV_MODE_1"" 3 ""DEV_MODE_2"" 4 ""ROLLS_MODE"" 5 ""DYNO_MODE"" 6 ""OFFROAD_ASSIST"" ;
VAL_ 545 VCFRONT_LVPowerStateIndex 0 ""Mux0"" 1 ""Mux1"" ;
VAL_ 545 VCFRONT_CMPDLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_amplifierLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_cpLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_das1HighCurrentLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_das2HighCurrentLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_difLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_dirLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_epasLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_espLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_hvacCompLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_hvcLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_iBoosterLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_ocsLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_oilPumpFrontLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_oilPumpRearLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_parkLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_pcsLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_ptcLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_radcLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_rcmLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_sccmLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_tasLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_tpmsLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_tunerLVRequest 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_uiHiCurrentLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_vcleftHiCurrentLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 545 VCFRONT_vcrightHiCurrentLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_amplifierLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_eFuseLockoutStatus 2 ""EFUSE_LOCKOUT_STATUS_ACTIVE"" 0 ""EFUSE_LOCKOUT_STATUS_IDLE"" 1 ""EFUSE_LOCKOUT_STATUS_PENDING"" ;
VAL_ 549 VCRIGHT_hvcLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_lumbarLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_ocsLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_ptcLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_rcmLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_rearOilPumpLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_tunerLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 549 VCRIGHT_vehiclePowerStateDBG 2 ""VEHICLE_POWER_STATE_ACCESSORY"" 1 ""VEHICLE_POWER_STATE_CONDITIONING"" 3 ""VEHICLE_POWER_STATE_DRIVE"" 0 ""VEHICLE_POWER_STATE_OFF"" ;
VAL_ 753 VCFRONT_eFuseDebugStatusIndex 8 ""VCF_DBG_STS_AUTOPILOT_1"" 9 ""VCF_DBG_STS_AUTOPILOT_2"" 4 ""VCF_DBG_STS_EPAS3P"" 5 ""VCF_DBG_STS_EPAS3S"" 6 ""VCF_DBG_STS_ESP_MOTOR"" 7 ""VCF_DBG_STS_ESP_VALVE"" 12 ""VCF_DBG_STS_HEADLAMPS"" 3 ""VCF_DBG_STS_IBOOSTER"" 18 ""VCF_DBG_STS_INVALID"" 17 ""VCF_DBG_STS_LV_BATTERY_DEBUG"" 16 ""VCF_DBG_STS_MISC_RAILS"" 2 ""VCF_DBG_STS_PCS"" 14 ""VCF_DBG_STS_PUMPS"" 15 ""VCF_DBG_STS_RAILS_A_B"" 10 ""VCF_DBG_STS_SLEEP_BYPASS"" 11 ""VCF_DBG_STS_UI"" 13 ""VCF_DBG_STS_VBAT_FUSED_HIGH_CURRENT"" 1 ""VCF_DBG_STS_VCLEFT"" 0 ""VCF_DBG_STS_VCRIGHT"" ;
VAL_ 753 VCFRONT_EPAS3PSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 753 VCFRONT_EPAS3PState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_EPAS3SSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 753 VCFRONT_EPAS3SState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_ESPMotorSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 753 VCFRONT_ESPMotorState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_ESPValveState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_PCSState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_autopilot1State 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_autopilot2State 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_headlampLeftTemperature 128 ""SNA"" ;
VAL_ 753 VCFRONT_headlampLeftVoltage 255 ""SNA"" ;
VAL_ 753 VCFRONT_headlampRightTemperature 128 ""SNA"" ;
VAL_ 753 VCFRONT_headlampRightVoltage 255 ""SNA"" ;
VAL_ 753 VCFRONT_iBoosterSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 753 VCFRONT_iBoosterState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_leftControllerState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_pcsSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 753 VCFRONT_rightControllerState 2 ""EFUSE_STATE_LOCKED_OUT"" 0 ""EFUSE_STATE_OFF"" 1 ""EFUSE_STATE_ON"" ;
VAL_ 753 VCFRONT_vbatFusedSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 753 VCFRONT_vcleftSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 753 VCFRONT_vcrightSelfTestResult 4 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_EFUSE_OUTPUT_SHORT"" 8 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_HIGH_MALFUNCTION"" 6 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_ENABLE_LOW_MALFUNCTION"" 10 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_NOT_LATCHED"" 7 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_CHANNEL_OPEN"" 5 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_POWER_FET_STUCK_ON"" 3 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_RAILS_UNSTABLE"" 9 ""EFUSE_SELF_TEST_EFUSE_RESULT_FAILED_TURN_OFF_PATH_TOO_SLOW"" 0 ""EFUSE_SELF_TEST_EFUSE_RESULT_NOT_RUN"" 2 ""EFUSE_SELF_TEST_EFUSE_RESULT_PASSED"" 1 ""EFUSE_SELF_TEST_EFUSE_RESULT_RUNNING"" 11 ""EFUSE_SELF_TEST_EFUSE_RESULT_SKIPPED"" ;
VAL_ 578 VCLEFT_cpLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 578 VCLEFT_diLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 578 VCLEFT_lumbarLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 578 VCLEFT_rcmLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 578 VCLEFT_sccmLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 578 VCLEFT_swcLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 578 VCLEFT_tpmsLVState 3 ""LV_FAULT"" 2 ""LV_GOING_DOWN"" 0 ""LV_OFF"" 1 ""LV_ON"" ;
VAL_ 578 VCLEFT_vehiclePowerStateDBG 2 ""VEHICLE_POWER_STATE_ACCESSORY"" 1 ""VEHICLE_POWER_STATE_CONDITIONING"" 3 ""VEHICLE_POWER_STATE_DRIVE"" 0 ""VEHICLE_POWER_STATE_OFF"" ;
VAL_ 579 VCRIGHT_hvacStatusIndex 3 ""END"" 0 ""STATUS_UI"" 1 ""STATUS_VCFRONT"" 2 ""STATUS_VCFRONT2"" ;
VAL_ 579 VCRIGHT_hvacACRunning 0 ""OFF"" 1 ""ON"" ;
VAL_ 579 VCRIGHT_hvacAirDistributionMode 4 ""DEFROST"" 5 ""DEFROST_FLOOR"" 6 ""DEFROST_PANEL"" 7 ""DEFROST_PANEL_FLOOR"" 1 ""FLOOR"" 0 ""NONE"" 2 ""PANEL"" 3 ""PANEL_FLOOR"" ;
VAL_ 579 VCRIGHT_hvacBlowerSegment 1 ""1"" 10 ""10"" 11 ""11"" 2 ""2"" 3 ""3"" 4 ""4"" 5 ""5"" 6 ""6"" 7 ""7"" 8 ""8"" 9 ""9"" 0 ""OFF"" ;
VAL_ 579 VCRIGHT_hvacDuctTargetLeft 255 ""SNA"" ;
VAL_ 579 VCRIGHT_hvacDuctTargetRight 255 ""SNA"" ;
VAL_ 579 VCRIGHT_hvacEvapInletTempEstimat 1023 ""SNA"" ;
VAL_ 579 VCRIGHT_hvacModelInitStatus 4 ""INIT_FORWARD_CALC"" 2 ""INIT_FROM_SENSORS"" 3 ""INIT_FROM_SENSORS_PREDICTION_ERROR"" 5 ""INIT_WAITING_FOR_SENSORS"" 1 ""NOT_INIT_WAIT_FOR_GTW"" 0 ""NOT_INIT_WAIT_FOR_SENSORS"" ;
VAL_ 579 VCRIGHT_hvacPowerState 0 ""OFF"" 1 ""ON"" 4 ""OVERHEAT_PROTECT"" 3 ""OVERHEAT_PROTECT_FANONLY"" 2 ""PRECONDITION"" ;
VAL_ 579 VCRIGHT_hvacRecirc 0 ""AUTO"" 2 ""FRESH"" 1 ""RECIRC"" ;
VAL_ 579 VCRIGHT_hvacSecondRowState 0 ""AUTO"" 4 ""HIGH"" 2 ""LOW"" 3 ""MED"" 1 ""OFF"" ;
VAL_ 579 VCRIGHT_hvacVentStatus 0 ""BOTH"" 1 ""LEFT"" 2 ""RIGHT"" ;
VAL_ 579 VCRIGHT_tempDuctLeft 255 ""SNA"" ;
VAL_ 579 VCRIGHT_tempDuctLeftLower 255 ""SNA"" ;
VAL_ 579 VCRIGHT_tempDuctLeftUpper 255 ""SNA"" ;
VAL_ 579 VCRIGHT_tempDuctRight 255 ""SNA"" ;
VAL_ 579 VCRIGHT_tempDuctRightLower 255 ""SNA"" ;
VAL_ 579 VCRIGHT_tempDuctRightUpper 255 ""SNA"" ;
VAL_ 524 VCRIGHT_hvacPerfTestState 2 ""BLOWING"" 0 ""STOPPED"" 1 ""WAITING"" ;
VAL_ 524 VCRIGHT_tempAmbientRaw 0 ""SNA"" ;
VAL_ 524 VCRIGHT_tempEvaporator 2047 ""SNA"" ;
VAL_ 524 VCRIGHT_tempEvaporatorTarget 255 ""SNA"" ;
VAL_ 737 VCFRONT_statusIndex 0 ""VCF_STS_IDX_BODY_CONTROLS"" 2 ""VCF_STS_IDX_HOMELINK"" 6 ""VCF_STS_IDX_INVALID"" 4 ""VCF_STS_IDX_LV_BATTERY"" 3 ""VCF_STS_IDX_REFRIGERANT_SYSTEM"" 5 ""VCF_STS_IDX_SYSTEM_HEALTH"" 1 ""VCF_STS_IDX_VEHICLE_STATE"" ;
VAL_ 737 VCFRONT_AS8510Voltage 4095 ""SNA"" ;
VAL_ 737 VCFRONT_airCompressorStatus 4 ""VCFRONT_AIR_COMPRESSOR_STATUS_FAULT"" 0 ""VCFRONT_AIR_COMPRESSOR_STATUS_OFF"" 1 ""VCFRONT_AIR_COMPRESSOR_STATUS_ON"" 5 ""VCFRONT_AIR_COMPRESSOR_STATUS_RETRY_AVAILABLE"" 7 ""VCFRONT_AIR_COMPRESSOR_STATUS_SNA"" 2 ""VCFRONT_AIR_COMPRESSOR_STATUS_TURNING_OFF"" 3 ""VCFRONT_AIR_COMPRESSOR_STATUS_TURNING_ON"" ;
VAL_ 737 VCFRONT_batterySMState 1 ""BATTERY_SM_STATE_CHARGE"" 2 ""BATTERY_SM_STATE_DISCHARGE"" 6 ""BATTERY_SM_STATE_DISCONNECTED_BATTERY_TEST"" 8 ""BATTERY_SM_STATE_FAULT"" 0 ""BATTERY_SM_STATE_INIT"" 5 ""BATTERY_SM_STATE_OTA_STANDBY"" 4 ""BATTERY_SM_STATE_RESISTANCE_ESTIMATION"" 7 ""BATTERY_SM_STATE_SHORTED_CELL_TEST"" 3 ""BATTERY_SM_STATE_STANDBY"" ;
VAL_ 737 VCFRONT_coolantFillRoutineStatus 3 ""FAULTED"" 1 ""MOVING_TO_FILL_POSITION"" 0 ""NOT_READY"" 2 ""READY_TO_FILL"" ;
VAL_ 737 VCFRONT_crashDetectedType 1 ""CRASH_DETECTED_TYPE_MINOR_1"" 2 ""CRASH_DETECTED_TYPE_MINOR_2"" 0 ""CRASH_DETECTED_TYPE_NONE"" 3 ""CRASH_DETECTED_TYPE_SEVERE"" ;
VAL_ 737 VCFRONT_crashState 0 ""CRASH_STATE_IDLE"" 1 ""CRASH_STATE_MINOR_1"" 2 ""CRASH_STATE_MINOR_2"" 3 ""CRASH_STATE_SEVERE"" ;
VAL_ 737 VCFRONT_frunkLatchStatus 5 ""LATCH_AJAR"" 2 ""LATCH_CLOSED"" 3 ""LATCH_CLOSING"" 7 ""LATCH_DEFAULT"" 8 ""LATCH_FAULT"" 1 ""LATCH_OPENED"" 4 ""LATCH_OPENING"" 0 ""LATCH_SNA"" 6 ""LATCH_TIMEOUT"" ;
VAL_ 737 VCFRONT_frunkLatchType 1 ""FRUNK_LATCH_TYPE_DOUBLE_ACTUATOR"" 2 ""FRUNK_LATCH_TYPE_DOUBLE_PULL"" 0 ""FRUNK_LATCH_TYPE_UNKNOWN"" ;
VAL_ 737 VCFRONT_homelinkCommStatus 3 ""HOMELINK_COMM_STATUS_FAULT"" 1 ""HOMELINK_COMM_STATUS_OFF"" 2 ""HOMELINK_COMM_STATUS_ON"" 0 ""HOMELINK_COMM_STATUS_SNA"" ;
VAL_ 737 VCFRONT_hvacPerfTestCommand 2 ""BLOW"" 1 ""INIT"" 0 ""NOT_STARTED"" 3 ""STOP"" ;
VAL_ 737 VCFRONT_iBoosterStateDBG 4 ""IBOOSTER_FORCE_OFF"" 2 ""IBOOSTER_GOING_DOWN"" 0 ""IBOOSTER_OFF"" 1 ""IBOOSTER_ON"" 3 ""IBOOSTER_WRITING_DATA_SHUTDOWN"" ;
VAL_ 737 VCFRONT_passengerBuckleStatus 1 ""BUCKLED"" 0 ""UNBUCKLED"" ;
VAL_ 737 VCFRONT_pressureRefrigDischarge 255 ""SNA"" ;
VAL_ 737 VCFRONT_pressureRefrigSuction 127 ""SNA"" ;
VAL_ 737 VCFRONT_radarHeaterState 4 ""HEATER_STATE_FAULT"" 2 ""HEATER_STATE_OFF"" 3 ""HEATER_STATE_OFF_UNAVAILABLE"" 1 ""HEATER_STATE_ON"" 0 ""HEATER_STATE_SNA"" ;
VAL_ 737 VCFRONT_refrigFillRoutineStatus 3 ""FAULTED"" 1 ""MOVING_TO_FILL_POSITION"" 0 ""NOT_READY"" 2 ""READY_TO_FILL"" ;
VAL_ 737 VCFRONT_vbatMonitorVoltage 4095 ""SNA"" ;
VAL_ 737 VCFRONT_vehicleStatusDBG 10 ""VEHICLE_STATUS_ACCESSORY"" 11 ""VEHICLE_STATUS_ACCESSORY_PLUS"" 3 ""VEHICLE_STATUS_BATTERY_POST_WAKE"" 12 ""VEHICLE_STATUS_CONDITIONING"" 14 ""VEHICLE_STATUS_CRASH"" 13 ""VEHICLE_STATUS_DRIVE"" 9 ""VEHICLE_STATUS_HV_UP_STANDBY"" 0 ""VEHICLE_STATUS_INIT"" 1 ""VEHICLE_STATUS_LOW_POWER_STANDBY"" 8 ""VEHICLE_STATUS_LV_AWAKE"" 7 ""VEHICLE_STATUS_LV_SHUTDOWN"" 15 ""VEHICLE_STATUS_OTA"" 17 ""VEHICLE_STATUS_RESET"" 2 ""VEHICLE_STATUS_SILENT_WAKE"" 5 ""VEHICLE_STATUS_SLEEP_SHUTDOWN"" 6 ""VEHICLE_STATUS_SLEEP_STANDBY"" 4 ""VEHICLE_STATUS_SYSTEM_CHECKS"" 16 ""VEHICLE_STATUS_TURN_ON_RAILS"" ;
VAL_ 737 VCFRONT_voltageProfile 3 ""VOLTAGE_PROFILE_ALWAYS_CLOSED_CONTACTORS"" 0 ""VOLTAGE_PROFILE_CHARGE"" 1 ""VOLTAGE_PROFILE_FLOAT"" 2 ""VOLTAGE_PROFILE_REDUCED_FLOAT"" ;
VAL_ 737 VCFRONT_wiperPosition 3 ""WIPER_POSITION_DELAYED_REST"" 2 ""WIPER_POSITION_DEPRESSED_PARK"" 1 ""WIPER_POSITION_SERVICE"" 0 ""WIPER_POSITION_SNA"" 4 ""WIPER_POSITION_WIPING"" ;
VAL_ 737 VCFRONT_wiperSpeed 2 ""WIPER_SPEED_1"" 3 ""WIPER_SPEED_2"" 4 ""WIPER_SPEED_3"" 5 ""WIPER_SPEED_4"" 6 ""WIPER_SPEED_5"" 8 ""WIPER_SPEED_HIGH"" 7 ""WIPER_SPEED_LOW"" 1 ""WIPER_SPEED_OFF"" 0 ""WIPER_SPEED_SNA"" ;
VAL_ 737 VCFRONT_wiperState 9 ""WIPER_STATE_CONT_FAST"" 10 ""WIPER_STATE_CONT_SLOW"" 3 ""WIPER_STATE_DELAYED_REST"" 2 ""WIPER_STATE_FAULT"" 7 ""WIPER_STATE_INTERMITTENT_HIGH"" 8 ""WIPER_STATE_INTERMITTENT_LOW"" 12 ""WIPER_STATE_INT_AUTO_HIGH"" 11 ""WIPER_STATE_INT_AUTO_LOW"" 6 ""WIPER_STATE_MOMENTARY_WIPE"" 4 ""WIPER_STATE_PARK"" 1 ""WIPER_STATE_SERVICE"" 0 ""WIPER_STATE_SNA"" 5 ""WIPER_STATE_WASH"" ;
VAL_ 897 VCFRONT_logging1HzIndex 18 ""BODY_CONTROL"" 0 ""COOLANT"" 19 ""COOLANT_2"" 2 ""COOLANT_VALVE"" 20 ""END"" 1 ""FAN_DEMAND_CONDENSER_AND_FET_TEMPS"" 3 ""HCML_LED_TEMPS"" 4 ""HCMR_LED_TEMPS"" 6 ""HEADLAMP_AIM"" 5 ""HOMELINK"" 15 ""HP_ARBITRATION"" 9 ""HP_CONTROL_LOOP_AND_STATE"" 10 ""HP_CYCLE_MODEL"" 8 ""HP_DATA_AND_ACCUMULATORS"" 12 ""HP_DISSIPATION_AND_POWER"" 11 ""HP_EXV_CALIBRATION"" 7 ""HP_EXV_RANGE"" 17 ""HP_MODE_OPTIONS_AND_ESTIMATES"" 16 ""HP_MODE_SELECT_AND_ESTIMATES"" 14 ""HP_PRESSURE_CONTROL"" 13 ""HP_TEMPS_AND_DEMANDS"" ;
VAL_ 897 VCFRONT_HCML_bladeTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCML_diffuseTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCML_highBeamTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCML_lowBeamSpotTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCML_turnTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCMR_bladeTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCMR_diffuseTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCMR_highBeamTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCMR_lowBeamSpotTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_HCMR_turnTemp 128 ""SNA"" ;
VAL_ 897 VCFRONT_coolantValveCountRange 1023 ""SNA"" ;
VAL_ 897 VCFRONT_coolantValveRadBypass 127 ""SNA"" ;
VAL_ 897 VCFRONT_coolantValveRecalReason 3 ""CALIBRATION_FAULT_NO_TRAVEL"" 2 ""GENERAL_FAULT"" 1 ""MAX_TRAVEL"" 5 ""MOTOR_FEEDBACK_INTERRUPTED"" 6 ""NVRAM_LOSS"" 4 ""SELF_TEST"" 7 ""SYSTEM_LEVEL_FAULT_RESPONSE"" 0 ""UNDEFINED"" ;
VAL_ 897 VCFRONT_drlMode 2 ""DRL_MODE_DRL"" 0 ""DRL_MODE_OFF"" 1 ""DRL_MODE_POSITION"" ;
VAL_ 897 VCFRONT_homelinkRegionCode 5 ""HOMELINK_REGION_CODE_AMERICAS"" 9 ""HOMELINK_REGION_CODE_CHINA"" 1 ""HOMELINK_REGION_CODE_EUROPE"" 8 ""HOMELINK_REGION_CODE_REST_OF_WORLD"" 0 ""HOMELINK_REGION_CODE_UNKNOWN"" ;
VAL_ 897 VCFRONT_hpMode 2 ""AMBIENT_SOURCE"" 15 ""BATTERY_COOL"" 17 ""BATTERY_COOL_CABIN_CONDENSER"" 16 ""BATTERY_COOL_CABIN_CONDENSER_REHEAT"" 18 ""BATTERY_COOL_CABIN_REHEAT"" 19 ""BATTERY_COOL_EVAPORATOR"" 13 ""BATTERY_HEAT_AMBIENT_SOURCE"" 14 ""BATTERY_HEAT_COP1"" 11 ""CABIN_COOL_EVAPORATOR"" 12 ""CABIN_COOL_EVAPORATOR_REHEAT"" 4 ""CABIN_HEAT_AMBIENT_SOURCE"" 10 ""CABIN_HEAT_BATTERY_COOL_REHEAT"" 9 ""CABIN_HEAT_BATTERY_HEAT_REHEAT_AMBIENT_SOURCE"" 7 ""CABIN_HEAT_BLEND"" 8 ""CABIN_HEAT_COP1"" 6 ""CABIN_HEAT_REHEAT_AMBIENT_SOURCE"" 5 ""CABIN_HEAT_REHEAT_SCAVENGE"" 3 ""CABIN_HEAT_SCAVENGE_ONLY"" 1 ""GENERAL"" 0 ""NONE"" ;
VAL_ 897 VCFRONT_hpRefrigerantPurgeState 2 ""COMPLETE"" 1 ""EVAP_PURGE"" 0 ""IDLE"" ;
VAL_ 897 VCFRONT_modeDesired 3 ""AMBIENT_SOURCE"" 2 ""BLEND"" 1 ""PARALLEL"" 0 ""SERIES"" ;
VAL_ 897 VCFRONT_modeTransitionID 23 ""ENTER_AMBIENTSOURCE"" 24 ""EXIT_AMBIENTSOURCE"" 20 ""INIT"" 21 ""OVERRIDE"" 6 ""PARALLEL_2_drive_batteryWantsHeat"" 7 ""PARALLEL_3_drive_batteryWantsCool"" 8 ""PARALLEL_4_drive_batteryNeedsCool"" 11 ""PARALLEL_5_charge_batteryWantsHeat"" 12 ""PARALLEL_6_charge_batteryWantsCool"" 15 ""PARALLEL_7_fastCharge_batteryWantsCool"" 16 ""PARALLEL_8_fastCharge_batteryWantsHeat"" 19 ""PARALLEL_9_drive_batteryThermalLimiting"" 0 ""PARALLEL_F1_noFlowRequest"" 33 ""PAR_1_drive_battNeedsActiveCooling_evapDisabled"" 34 ""PAR_2_drive_ptNeedsActiveCooling"" 35 ""PAR_3_drive_chillerPassivelyCoolsBatt"" 36 ""PAR_4_drive_cannotPassivelyCoolBatt"" 37 ""PAR_5_drive_battAboveHotStagnationTemp"" 38 ""PAR_6_FC_battNeedsActiveCooling_evapDisabled"" 39 ""PAR_7_FC_battNeedsActiveCooling_evapEnabled"" 40 ""PAR_8_charge_battAbovePassiveTarget"" 3 ""SERIES_1_drive_batteryWantsCool"" 4 ""SERIES_2_drive_batteryNeedsHeat"" 5 ""SERIES_3_drive_batteryWantsHeat"" 9 ""SERIES_4_charge_batteryNeedsHeat"" 10 ""SERIES_5_charge_batteryWantsHeat"" 13 ""SERIES_6_fastCharge_batteryNeedsHeat"" 14 ""SERIES_7_fastCharge_batteryWantsCool"" 17 ""SERIES_8_preConditioning_batteryNeedsHeat"" 18 ""SERIES_9_drive_driveUnitThermalLimiting"" 1 ""SERIES_F2_faultPumps"" 2 ""SERIES_F3_faultTempSensors"" 25 ""SER_1_drive_battNeedsActiveCooling_evapEnabled"" 26 ""SER_2_drive_battBelowHotStagnationTemp"" 27 ""SER_3_drive_chillerPassivelyCools"" 28 ""SER_4_drive_radPassivelyCoolsBatt"" 29 ""SER_5_FC_battHeatingNeeded"" 30 ""SER_6_FC_battNeedsActiveCooling_evapDisabled"" 31 ""SER_7_FC_battNeedsActiveCooling_evapEnabled"" 32 ""SER_8_charge_battBelowPassiveTarget"" 22 ""UNDEFINED"" ;
VAL_ 897 VCFRONT_passiveCoolingState 3 ""CannotCoolBattery"" 2 ""ChillerAndRadCoolSeriesLoop"" 1 ""ChillerCoolsParallelBattLoop"" 0 ""ChillerCoolsSeriesLoop"" ;
VAL_ 897 VCFRONT_pumpBatteryFETTemp 255 ""SNA"" ;
VAL_ 897 VCFRONT_pumpPowertrainFETTemp 255 ""SNA"" ;
VAL_ 897 VCFRONT_radiatorFanFETTemp 255 ""SNA"" ;
VAL_ 897 VCFRONT_radiatorFanRunReason 1 ""ACTIVE_MANAGER"" 2 ""AMBIENT_SNIFF"" 5 ""COAST_MODE"" 4 ""HEAT_PUMP"" 6 ""MIN_ON_GLOBAL"" 7 ""MIN_ON_NVH"" 0 ""NONE"" 3 ""NVH_MASKING"" 8 ""UDS"" ;
VAL_ 897 VCFRONT_tempRefrigSuction 255 ""SNA"" ;
VAL_ 553 GearLeverPosition229 4 ""Full Up"" 3 ""Half Up"" 2 ""Full Down"" 1 ""Half Down"" 0 ""Center"" ;
VAL_ 585 SCCM_highBeamStalkStatus 0 ""IDLE"" 1 ""PULL"" 2 ""PUSH"" 3 ""SNA"" ;
VAL_ 585 SCCM_turnIndicatorStalkStatus 5 ""DOWN_0_5"" 6 ""DOWN_1"" 7 ""DOWN_1_5"" 8 ""DOWN_2"" 0 ""IDLE"" 9 ""SNA"" 1 ""UP_0_5"" 2 ""UP_1"" 3 ""UP_1_5"" 4 ""UP_2"" ;
VAL_ 585 SCCM_washWipeButtonStatus 1 ""1ST_DETENT"" 2 ""2ND_DETENT"" 0 ""NOT_PRESSED"" 3 ""SNA"" ;
VAL_ 390 DIF_axleSpeed 32768 ""SNA"" ;
VAL_ 390 DIF_slavePedalPos 255 ""SNA"" ;
VAL_ 390 DIF_torqueActual 4096 ""SNA"" ;
VAL_ 390 DIF_torqueCommand 4096 ""SNA"" ;
VAL_ 918 FrontOilPumpState396 7 ""OIL_PUMP_SNA"" 6 ""OIL_PUMP_FAULTED"" 2 ""OIL_PUMP_COLD_STARTUP"" 1 ""OIL_PUMP_ENABLE"" 0 ""OIL_PUMP_STANDBY"" ;
VAL_ 917 DIR_oilPumpFluidTQF 1 ""OIL_PUMP_FLUIDT_HIGH_CONFIDENCE"" 0 ""OIL_PUMP_FLUIDT_LOW_CONFIDENCE"" ;
VAL_ 917 DIR_oilPumpState 2 ""OIL_PUMP_COLD_STARTUP"" 1 ""OIL_PUMP_ENABLE"" 6 ""OIL_PUMP_FAULTED"" 7 ""OIL_PUMP_SNA"" 0 ""OIL_PUMP_STANDBY"" ;
VAL_ 341 ESP_WheelRotationReR 3 ""WR_NOT_DEFINABLE"" 2 ""WR_STANDSTILL"" 1 ""WR_BACKWARD"" 0 ""WR_FORWARD"" ;
VAL_ 341 ESP_WheelRotationReL 3 ""WR_NOT_DEFINABLE"" 2 ""WR_STANDSTILL"" 1 ""WR_BACKWARD"" 0 ""WR_FORWARD"" ;
VAL_ 341 ESP_WheelRotationFrR 3 ""WR_NOT_DEFINABLE"" 2 ""WR_STANDSTILL"" 1 ""WR_BACKWARD"" 0 ""WR_FORWARD"" ;
VAL_ 341 ESP_WheelRotationFrL 3 ""WR_NOT_DEFINABLE"" 2 ""WR_STANDSTILL"" 1 ""WR_BACKWARD"" 0 ""WR_FORWARD"" ;
VAL_ 341 ESP_wheelSpeedsQF 1 ""ALL_WSS_VALID"" 0 ""ONE_OR_MORE_WSS_INVALID"" ;
VAL_ 341 ESP_vehicleStandstillSts 1 ""STANDSTILL"" 0 ""NOT_STANDSTILL"" ;
VAL_ 341 ESP_vehicleSpeed 1023 ""ESP_VEHICLE_SPEED_SNA"" ;
VAL_ 373 WheelSpeedRR175 8191 ""SNA"" ;
VAL_ 373 WheelSpeedRL175 8191 ""SNA"" ;
VAL_ 373 WheelSpeedFR175 8191 ""SNA"" ;
VAL_ 373 WheelSpeedFL175 8191 ""SNA"" ;
VAL_ 389 ESP_brakeTorqueFrL 4095 ""SNA"" ;
VAL_ 389 ESP_brakeTorqueFrR 4095 ""SNA"" ;
VAL_ 389 ESP_brakeTorqueReL 4095 ""SNA"" ;
VAL_ 389 ESP_brakeTorqueReR 4095 ""SNA"" ;
VAL_ 389 ESP_brakeTorqueQF 1 ""IN_SPEC"" 0 ""UNDEFINABLE_ACCURACY"" ;
VAL_ 962 VCLEFT_switchStatusIndex 0 ""VCLEFT_SWITCH_STATUS_INDEX_0"" 1 ""VCLEFT_SWITCH_STATUS_INDEX_1"" 2 ""VCLEFT_SWITCH_STATUS_INDEX_INVALID"" ;
VAL_ 962 VCLEFT_frontBuckleSwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontOccupancySwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatBackrestBack 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatBackrestForward 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatLiftDown 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatLiftUp 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatLumbarDown 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatLumbarIn 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatLumbarOut 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatLumbarUp 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatTiltDown 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatTiltUp 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatTrackBack 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_frontSeatTrackForward 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_rearCenterBuckleSwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_rearCenterOccupancySwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_rearLeftBuckleSwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_rearLeftOccupancySwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_rearRightOccupancySwitch 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_rightMirrorTilt 1 ""MIRROR_TILT_DOWN"" 4 ""MIRROR_TILT_LEFT"" 3 ""MIRROR_TILT_RIGHT"" 0 ""MIRROR_TILT_STOP"" 2 ""MIRROR_TILT_UP"" ;
VAL_ 962 VCLEFT_swcLeftPressed 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_swcLeftTiltLeft 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_swcLeftTiltRight 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_swcRightPressed 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_swcRightTiltLeft 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 962 VCLEFT_swcRightTiltRight 3 ""SWITCH_FAULT"" 1 ""SWITCH_OFF"" 2 ""SWITCH_ON"" 0 ""SWITCH_SNA"" ;
VAL_ 822 DI_performancePackage 0 ""BASE"" 3 ""BASE_PLUS"" 1 ""PERFORMANCE"" 4 ""SNA"" ;
VAL_ 659 UI_accOvertakeEnable 0 ""ACC_OVERTAKE_OFF"" 1 ""ACC_OVERTAKE_ON"" 3 ""SNA"" ;
VAL_ 659 UI_aebEnable 0 ""AEB_OFF"" 1 ""AEB_ON"" 3 ""SNA"" ;
VAL_ 659 UI_aesEnable 0 ""AES_OFF"" 1 ""AES_ON"" 3 ""SNA"" ;
VAL_ 659 UI_ahlbEnable 0 ""AHLB_OFF"" 1 ""AHLB_ON"" 3 ""SNA"" ;
VAL_ 659 UI_autoLaneChangeEnable 0 ""OFF"" 1 ""ON"" 3 ""SNA"" ;
VAL_ 659 UI_autoParkRequest 7 ""ABORT"" 8 ""COMPLETE"" 0 ""NONE"" 5 ""PARALLEL_PULL_OUT_TO_LEFT"" 6 ""PARALLEL_PULL_OUT_TO_RIGHT"" 2 ""PARK_LEFT_CROSS"" 1 ""PARK_LEFT_PARALLEL"" 4 ""PARK_RIGHT_CROSS"" 3 ""PARK_RIGHT_PARALLEL"" 10 ""PAUSE"" 11 ""RESUME"" 9 ""SEARCH"" 15 ""SNA"" ;
VAL_ 659 UI_bsdEnable 0 ""BSD_OFF"" 1 ""BSD_ON"" 3 ""SNA"" ;
VAL_ 659 UI_distanceUnits 0 ""DISTANCEUNITS_KM"" 1 ""DISTANCEUNITS_MILES"" ;
VAL_ 659 UI_fcwEnable 0 ""FCW_OFF"" 1 ""FCW_ON"" 3 ""SNA"" ;
VAL_ 659 UI_fcwSensitivity 1 ""AEB_SENSITIVITY_AVERAGE"" 0 ""AEB_SENSITIVITY_EARLY"" 2 ""AEB_SENSITIVITY_LATE"" 3 ""SNA"" ;
VAL_ 659 UI_latControlEnable 0 ""LATERAL_CONTROL_OFF"" 1 ""LATERAL_CONTROL_ON"" 3 ""LATERAL_CONTROL_SNA"" 2 ""LATERAL_CONTROL_UNAVAILABLE"" ;
VAL_ 659 UI_ldwEnable 1 ""LDW_TRIGGERS_HAPTIC"" 0 ""NO_HAPTIC"" 3 ""SNA"" ;
VAL_ 659 UI_parkBrakeRequest 0 ""PARK_BRAKE_REQUEST_IDLE"" 1 ""PARK_BRAKE_REQUEST_PRESSED"" 3 ""PARK_BRAKE_REQUEST_SNA"" ;
VAL_ 659 UI_pedalSafetyEnable 0 ""PEDAL_SAFETY_OFF"" 1 ""PEDAL_SAFETY_ON"" 3 ""SNA"" ;
VAL_ 659 UI_redLightStopSignEnable 0 ""RLSSW_OFF"" 1 ""RLSSW_ON"" 3 ""SNA"" ;
VAL_ 659 UI_selfParkTune 15 ""SNA"" ;
VAL_ 659 UI_steeringTuneRequest 0 ""STEERING_TUNE_COMFORT"" 2 ""STEERING_TUNE_SPORT"" 1 ""STEERING_TUNE_STANDARD"" ;
VAL_ 659 UI_tractionControlMode 2 ""TC_DEV_MODE_1_SELECTED"" 3 ""TC_DEV_MODE_2_SELECTED"" 5 ""TC_DYNO_MODE_SELECTED"" 0 ""TC_NORMAL_SELECTED"" 6 ""TC_OFFROAD_ASSIST_SELECTED"" 4 ""TC_ROLLS_MODE_SELECTED"" 1 ""TC_SLIP_START_SELECTED"" ;
VAL_ 659 UI_trailerMode 0 ""TRAILER_MODE_OFF"" 1 ""TRAILER_MODE_ON"" ;
VAL_ 659 UI_winchModeRequest 1 ""WINCH_MODE_ENTER"" 2 ""WINCH_MODE_EXIT"" 0 ""WINCH_MODE_IDLE"" ;
VAL_ 659 UI_zeroSpeedConfirmed 0 ""ZERO_SPEED_CANCELED"" 1 ""ZERO_SPEED_CONFIRMED"" 3 ""ZERO_SPEED_SNA"" 2 ""ZERO_SPEED_UNUSED"" ;
VAL_ 616 DI_primaryUnitSiliconType 1 ""IGBT"" 0 ""MOSFET"" ;
VAL_ 532 FC_postID 2 ""FC_POST_ID_2"" 3 ""FC_POST_ID_SNA"" 0 ""FC_POST_MASTER"" 1 ""FC_POST_SLAVE"" ;
VAL_ 532 FC_statusCode 3 ""FC_STATUS_DEPRECATED_3"" 4 ""FC_STATUS_DEPRECATED_4"" 6 ""FC_STATUS_EXT_ISOACTIVE"" 5 ""FC_STATUS_INT_ISOACTIVE"" 14 ""FC_STATUS_MALFUNCTION"" 15 ""FC_STATUS_NODATA"" 13 ""FC_STATUS_NOTCOMPATIBLE"" 0 ""FC_STATUS_NOTREADY_SNA"" 7 ""FC_STATUS_POST_OUT_OF_SERVICE"" 1 ""FC_STATUS_READY"" 2 ""FC_STATUS_UPDATE_IN_PROGRESS"" ;
VAL_ 532 FC_type 3 ""FC_TYPE_CC_EVSE"" 1 ""FC_TYPE_CHADEMO"" 4 ""FC_TYPE_COMBO"" 2 ""FC_TYPE_GB"" 5 ""FC_TYPE_MC_EVSE"" 6 ""FC_TYPE_OTHER"" 7 ""FC_TYPE_SNA"" 0 ""FC_TYPE_SUPERCHARGER"" ;
VAL_ 535 FC_status3DataSelect 0 ""Mux0"" 1 ""Mux1"" ;
VAL_ 535 FC_brand 0 ""FC_BRAND_SNA"" 1 ""FC_BRAND_TESLA"" ;
VAL_ 535 FC_class 0 ""FC_CLASS_SNA"" 1 ""FC_CLASS_SUPERCHARGER"" 2 ""FC_CLASS_URBANCHARGER"" ;
VAL_ 535 FC_coolingType 2 ""FC_COOLING_TYPE_CONVECTION"" 1 ""FC_COOLING_TYPE_LIQUID"" 0 ""FC_COOLING_TYPE_SNA"" ;
VAL_ 535 FC_generation 0 ""GENERATION_SNA"" ;
VAL_ 535 FC_uiStopType 2 ""FC_UI_STOP_TYPE_MOMENTARY"" 0 ""FC_UI_STOP_TYPE_SNA"" 1 ""FC_UI_STOP_TYPE_TOGGLE"" ;
VAL_ 801 VCFRONT_brakeFluidLevel 1 ""LOW"" 2 ""NORMAL"" 0 ""SNA"" ;
VAL_ 801 VCFRONT_coolantLevel 1 ""FILLED"" 0 ""NOT_OK"" ;
VAL_ 801 VCFRONT_tempAmbient 0 ""SNA"" ;
VAL_ 801 VCFRONT_tempAmbientFiltered 0 ""SNA"" ;
VAL_ 801 VCFRONT_tempCoolantBatInlet 1023 ""SNA"" ;
VAL_ 801 VCFRONT_tempCoolantPTInlet 2047 ""SNA"" ;
VAL_ 801 VCFRONT_washerFluidLevel 1 ""LOW"" 2 ""NORMAL"" 0 ""SNA"" ;
VAL_ 769 VCFRONT_infoIndex 13 ""BC_INFO_APP_CRC"" 17 ""BC_INFO_APP_GITHASH"" 15 ""BC_INFO_BOOTLOADER_CRC"" 18 ""BC_INFO_BOOTLOADER_GITHASH"" 14 ""BC_INFO_BOOTLOADER_SVN"" 10 ""BC_INFO_BUILD_HWID_COMPONENTID"" 0 ""BC_INFO_DEPRECATED_0"" 1 ""BC_INFO_DEPRECATED_1"" 2 ""BC_INFO_DEPRECATED_2"" 3 ""BC_INFO_DEPRECATED_3"" 4 ""BC_INFO_DEPRECATED_4"" 5 ""BC_INFO_DEPRECATED_5"" 6 ""BC_INFO_DEPRECATED_6"" 7 ""BC_INFO_DEPRECATED_7"" 8 ""BC_INFO_DEPRECATED_8"" 9 ""BC_INFO_DEPRECATED_9"" 255 ""BC_INFO_END"" 11 ""BC_INFO_PCBAID_ASSYID_USAGEID"" 16 ""BC_INFO_SUBCOMPONENT1"" 23 ""BC_INFO_SUBCOMPONENT2"" 24 ""BC_INFO_SUBCOMPONENT3"" 31 ""BC_INFO_SUBCOMPONENT4"" 32 ""BC_INFO_SUBCOMPONENT5"" 33 ""BC_INFO_SUBCOMPONENT6"" 20 ""BC_INFO_UDS_PROTOCOL_BOOTCRC"" 19 ""BC_INFO_VERSION_DEPRECATED"" ;
VAL_ 769 VCFRONT_infoAssemblyId 1 ""ASSEMBLY1"" 255 ""ASSEMBLY_SNA"" ;
VAL_ 769 VCFRONT_infoBuildType 2 ""INFO_LOCAL_BUILD"" 4 ""INFO_MFG_BUILD"" 1 ""INFO_PLATFORM_BUILD"" 3 ""INFO_TRACEABLE_CI_BUILD"" 0 ""INFO_UNKNOWN_BUILD"" ;
VAL_ 513 VCFRONT_loggingAndVitals10HzInde 7 ""END"" 3 ""EXV_FLOW"" 5 ""EXV_FLOW_TARGET"" 6 ""EXV_STATE"" 4 ""HP_STATE"" 2 ""STATES_AND_SENSORS"" 0 ""TARGETS_AND_ACTUALS_0"" 1 ""TARGETS_SENSORS_AND_ACTUALS_1"" ;
VAL_ 513 VCFRONT_cclExvState 8 ""CALIB_CLOSE"" 9 ""CALIB_CLOSE_OVERDRIVE"" 4 ""FAULTED"" 2 ""INIT_CLOSE"" 1 ""INIT_OPEN"" 6 ""OVERDRIVING_SHUT"" 3 ""READY"" 7 ""READY_SHUT"" 0 ""UNINIT"" 5 ""WAIT"" ;
VAL_ 513 VCFRONT_ccrExvState 8 ""CALIB_CLOSE"" 9 ""CALIB_CLOSE_OVERDRIVE"" 4 ""FAULTED"" 2 ""INIT_CLOSE"" 1 ""INIT_OPEN"" 6 ""OVERDRIVING_SHUT"" 3 ""READY"" 7 ""READY_SHUT"" 0 ""UNINIT"" 5 ""WAIT"" ;
VAL_ 513 VCFRONT_chillerExvState 8 ""CALIB_CLOSE"" 9 ""CALIB_CLOSE_OVERDRIVE"" 4 ""FAULTED"" 2 ""INIT_CLOSE"" 1 ""INIT_OPEN"" 6 ""OVERDRIVING_SHUT"" 3 ""READY"" 7 ""READY_SHUT"" 0 ""UNINIT"" 5 ""WAIT"" ;
VAL_ 513 VCFRONT_compressorState 3 ""FAULT"" 1 ""READY"" 2 ""RUNNING"" 0 ""STANDBY"" ;
VAL_ 513 VCFRONT_coolantValveMode 3 ""AMBIENT_SOURCE"" 2 ""BLEND"" 1 ""PARALLEL"" 0 ""SERIES"" ;
VAL_ 513 VCFRONT_evapExvState 8 ""CALIB_CLOSE"" 9 ""CALIB_CLOSE_OVERDRIVE"" 4 ""FAULTED"" 2 ""INIT_CLOSE"" 1 ""INIT_OPEN"" 6 ""OVERDRIVING_SHUT"" 3 ""READY"" 7 ""READY_SHUT"" 0 ""UNINIT"" 5 ""WAIT"" ;
VAL_ 513 VCFRONT_exvState 8 ""CALIB_CLOSE"" 9 ""CALIB_CLOSE_OVERDRIVE"" 4 ""FAULTED"" 2 ""INIT_CLOSE"" 1 ""INIT_OPEN"" 6 ""OVERDRIVING_SHUT"" 3 ""READY"" 7 ""READY_SHUT"" 0 ""UNINIT"" 5 ""WAIT"" ;
VAL_ 513 VCFRONT_hpBatteryLoadType 2 ""BATT_COOL"" 1 ""BATT_HEAT"" 0 ""NONE"" ;
VAL_ 513 VCFRONT_hpBlendType 2 ""HP_FULL"" 0 ""HP_NONE"" 1 ""HP_PARTIAL"" ;
VAL_ 513 VCFRONT_hpCabinLoadType 1 ""CC"" 3 ""EVAP"" 0 ""NONE"" 2 ""REHEAT"" ;
VAL_ 513 VCFRONT_hpDominantLoad 5 ""CC"" 2 ""CHILLER"" 1 ""EVAP"" 6 ""HIGH_BOTH"" 4 ""LCC"" 3 ""LOW_BOTH"" 0 ""NONE"" ;
VAL_ 513 VCFRONT_hpHighSideHX 3 ""BOTH"" 2 ""CC"" 1 ""LCC"" 0 ""NONE"" ;
VAL_ 513 VCFRONT_hpLowSideHX 3 ""BOTH"" 1 ""CHILLER"" 2 ""EVAP"" 0 ""NONE"" ;
VAL_ 513 VCFRONT_hpReqCoolantMode 4 ""AMBIENT_SOURCE"" 0 ""ANY"" 3 ""PARALLEL"" 2 ""SERIES_BYPASS"" 1 ""SERIES_NO_BYPASS"" ;
VAL_ 513 VCFRONT_lccExvState 8 ""CALIB_CLOSE"" 9 ""CALIB_CLOSE_OVERDRIVE"" 4 ""FAULTED"" 2 ""INIT_CLOSE"" 1 ""INIT_OPEN"" 6 ""OVERDRIVING_SHUT"" 3 ""READY"" 7 ""READY_SHUT"" 0 ""UNINIT"" 5 ""WAIT"" ;
VAL_ 513 VCFRONT_pressureRefrigDischargeV 255 ""SNA"" ;
VAL_ 513 VCFRONT_pressureRefrigLiquid 255 ""SNA"" ;
VAL_ 513 VCFRONT_pressureRefrigSuctionVit 127 ""SNA"" ;
VAL_ 513 VCFRONT_pumpBatteryRPMActual 1023 ""SNA"" ;
VAL_ 513 VCFRONT_pumpPowertrainRPMActual 1023 ""SNA"" ;
VAL_ 513 VCFRONT_radiatorFanRPMActual 1023 ""SNA"" ;
VAL_ 513 VCFRONT_recircExvState 8 ""CALIB_CLOSE"" 9 ""CALIB_CLOSE_OVERDRIVE"" 4 ""FAULTED"" 2 ""INIT_CLOSE"" 1 ""INIT_OPEN"" 6 ""OVERDRIVING_SHUT"" 3 ""READY"" 7 ""READY_SHUT"" 0 ""UNINIT"" 5 ""WAIT"" ;
VAL_ 513 VCFRONT_solenoidEvapState 2 ""CLOSED"" 3 ""FAULTED"" 1 ""OPENED"" 0 ""UNDEFINED"" ;
VAL_ 513 VCFRONT_tempRefrigDischarge 2047 ""SNA"" ;
VAL_ 513 VCFRONT_tempRefrigLiquid 2047 ""SNA"" ;
VAL_ 513 VCFRONT_tempSuperheatActual 1023 ""SNA"" ;
VAL_ 548 DCDCstate224 6 ""Error"" 5 ""Shutdown"" 4 ""HVactive"" 3 ""Precharge"" 2 ""PrechargeStart"" 1 ""12vChg"" 0 ""Idle"" ;
VAL_ 548 DCDCSubState224 0 ""PWR_UP_INIT"" 1 ""STANDBY"" 2 ""12V_SUPPORT_ACTIVE"" 3 ""DIS_HVBUS"" 4 ""PCHG_FAST_DIS_HVBUS"" 5 ""PCHG_SLOW_DIS_HVBUS"" 6 ""PCHG_DWELL_CHARGE"" 7 ""PCHG_DWELL_WAIT"" 8 ""PCHG_DI_RECOVERY_WAIT"" 9 ""PCHG_ACTIVE"" 10 ""PCHG_FLT_FAST_DIS_HVBUS"" 11 ""SHUTDOWN"" 12 ""12V_SUPPORT_FAULTED"" 13 ""DIS_HVBUS_FAULTED"" 14 ""PCHG_FAULTED"" 15 ""CLEAR_FAULTS"" 16 ""FAULTED"" 17 ""NUM"" ;
VAL_ 551 CMP_state 3 ""CMP_STATE_FAULT"" 0 ""CMP_STATE_NONE"" 1 ""CMP_STATE_NORMAL"" 15 ""CMP_STATE_SNA"" 5 ""CMP_STATE_SOFT_SHUTDOWN"" 4 ""CMP_STATE_SOFT_START"" 2 ""CMP_STATE_WAIT"" ;
VAL_ 280 DI_accelPedalPos 255 ""SNA"" ;
VAL_ 280 DI_brakePedalState 2 ""INVALID"" 0 ""OFF"" 1 ""ON"" ;
VAL_ 280 DI_driveBlocked 1 ""DRIVE_BLOCKED_FRUNK"" 0 ""DRIVE_BLOCKED_NONE"" 2 ""DRIVE_BLOCKED_PROX"" ;
VAL_ 280 DI_epbRequest 0 ""DI_EPBREQUEST_NO_REQUEST"" 1 ""DI_EPBREQUEST_PARK"" 2 ""DI_EPBREQUEST_UNPARK"" ;
VAL_ 280 DI_gear 4 ""DI_GEAR_D"" 0 ""DI_GEAR_INVALID"" 3 ""DI_GEAR_N"" 1 ""DI_GEAR_P"" 2 ""DI_GEAR_R"" 7 ""DI_GEAR_SNA"" ;
VAL_ 280 DI_immobilizerState 2 ""DI_IMM_STATE_AUTHENTICATING"" 3 ""DI_IMM_STATE_DISARMED"" 6 ""DI_IMM_STATE_FAULT"" 4 ""DI_IMM_STATE_IDLE"" 0 ""DI_IMM_STATE_INIT_SNA"" 1 ""DI_IMM_STATE_REQUEST"" 5 ""DI_IMM_STATE_RESET"" ;
VAL_ 280 DI_keepDrivePowerStateRequest 1 ""KEEP_ALIVE"" 0 ""NO_REQUEST"" ;
VAL_ 280 DI_systemState 4 ""DI_SYS_ABORT"" 5 ""DI_SYS_ENABLE"" 3 ""DI_SYS_FAULT"" 1 ""DI_SYS_IDLE"" 2 ""DI_SYS_STANDBY"" 0 ""DI_SYS_UNAVAILABLE"" ;
VAL_ 280 DI_trackModeState 1 ""TRACK_MODE_AVAILABLE"" 2 ""TRACK_MODE_ON"" 0 ""TRACK_MODE_UNAVAILABLE"" ;
VAL_ 280 DI_tractionControlMode 6 ""Offroad Assist"" 5 ""Dyno Mode"" 4 ""Rolls Mode"" 3 ""Dev2"" 2 ""Dev1"" 1 ""Slip Start"" 0 ""Standard"" ;
VAL_ 850 BMS_energyBuffer 255 ""SNA"" ;
VAL_ 850 BMS_energyToChargeComplete 2047 ""SNA"" ;
VAL_ 850 BMS_expectedEnergyRemaining 2047 ""SNA"" ;
VAL_ 850 BMS_idealEnergyRemaining 2047 ""SNA"" ;
VAL_ 850 BMS_nominalEnergyRemaining 2047 ""SNA"" ;
VAL_ 850 BMS_nominalFullPackEnergy 2047 ""SNA"" ;
VAL_ 914 BMS_moduleType 0 ""UNKNOWN"" 1 ""E3_NCT"" 2 ""E1_NCT"" 3 ""E3_CT"" 4 ""E1_CT"" 5 ""E1_CP"" ;
VAL_ 914 BMS_packConfigMultiplexer 0 ""Mux0"" 1 ""Mux1"" ;
VAL_ 914 BMS_reservedConfig_0 0 ""BMS_CONFIG_0"" 1 ""BMS_CONFIG_1"" 10 ""BMS_CONFIG_10"" 11 ""BMS_CONFIG_11"" 12 ""BMS_CONFIG_12"" 13 ""BMS_CONFIG_13"" 14 ""BMS_CONFIG_14"" 15 ""BMS_CONFIG_15"" 16 ""BMS_CONFIG_16"" 17 ""BMS_CONFIG_17"" 18 ""BMS_CONFIG_18"" 19 ""BMS_CONFIG_19"" 2 ""BMS_CONFIG_2"" 20 ""BMS_CONFIG_20"" 21 ""BMS_CONFIG_21"" 22 ""BMS_CONFIG_22"" 23 ""BMS_CONFIG_23"" 24 ""BMS_CONFIG_24"" 25 ""BMS_CONFIG_25"" 26 ""BMS_CONFIG_26"" 27 ""BMS_CONFIG_27"" 28 ""BMS_CONFIG_28"" 29 ""BMS_CONFIG_29"" 3 ""BMS_CONFIG_3"" 30 ""BMS_CONFIG_30"" 31 ""BMS_CONFIG_31"" 4 ""BMS_CONFIG_4"" 5 ""BMS_CONFIG_5"" 6 ""BMS_CONFIG_6"" 7 ""BMS_CONFIG_7"" 8 ""BMS_CONFIG_8"" 9 ""BMS_CONFIG_9"" ;
VAL_ 594 BMS_powerLimitsState 1 ""POWER_CALCULATED_FOR_DRIVE"" 0 ""POWER_NOT_CALCULATED_FOR_DRIVE"" ;
VAL_ 658 BattBeginningOfLifeEnergy292 1023 ""SNA"" ;
VAL_ 658 BMS_battTempPct 255 ""SNA"" ;
VAL_ 599 DI_uiSpeed 511 ""DI_UI_SPEED_SNA"" ;
VAL_ 599 DI_uiSpeedHighSpeed 511 ""DI_UI_HIGH_SPEED_SNA"" ;
VAL_ 599 DI_uiSpeedUnits 1 ""DI_SPEED_KPH"" 0 ""DI_SPEED_MPH"" ;
VAL_ 599 DI_vehicleSpeed 4095 ""SNA"" ;
VAL_ 680 CMPD_state 3 ""CMPD_STATE_FAULT"" 4 ""CMPD_STATE_IDLE"" 0 ""CMPD_STATE_INIT"" 1 ""CMPD_STATE_RUNNING"" 15 ""CMPD_STATE_SNA"" 2 ""CMPD_STATE_STANDBY"" ;
VAL_ 680 CMPD_wasteHeatState 1 ""CMPD_WASTE_HEAT_STATE_ACTIVE"" 2 ""CMPD_WASTE_HEAT_STATE_NOT_AVAILABLE"" 0 ""CMPD_WASTE_HEAT_STATE_OFF"" 3 ""CMPD_WASTE_HEAT_STATE_UNUSED"" ;
VAL_ 886 DIF_inverterTQF 0 ""INVERTERT_INIT"" 1 ""INVERTERT_IRRATIONAL"" 2 ""INVERTERT_RATIONAL"" 3 ""INVERTERT_UNKNOWN"" ;
VAL_ 789 DIR_inverterTQF 0 ""INVERTERT_INIT"" 1 ""INVERTERT_IRRATIONAL"" 2 ""INVERTERT_RATIONAL"" 3 ""INVERTERT_UNKNOWN"" ;
VAL_ 950 Odometer3B6 4294967 ""SNA"" ;
VAL_ 264 DIR_axleSpeed 32768 ""SNA"" ;
VAL_ 264 DIR_slavePedalPos 255 ""SNA"" ;
VAL_ 264 DIR_torqueActual 4096 ""SNA"" ;
VAL_ 264 DIR_torqueCommand 4096 ""SNA"" ;
VAL_ 294 DIR_vBatQF 0 ""NOT_QUALIFIED"" 1 ""QUALIFIED"" ;
VAL_ 421 DIF_vBatQF 0 ""NOT_QUALIFIED"" 1 ""QUALIFIED"" ;
VAL_ 552 EPBResmCaliperRequest228 3 ""disengaging"" 2 ""engaging"" 1 ""idle"" ;
VAL_ 552 EPBResmOperationTrigger228 22 ""SuperPark"" 6 ""Released"" 1 ""ParkEngaged"" ;
VAL_ 552 EPBRunitStatus228 10 ""Disengaging"" 8 ""Engaging"" 3 ""ParkEngaged"" 1 ""DriveReleased"" ;
VAL_ 648 EPBLesmCaliperRequest288 3 ""disengaging"" 2 ""engaging"" 1 ""idle"" ;
VAL_ 648 EPBLesmOperationTrigger288 22 ""SuperPark"" 6 ""Released"" 1 ""ParkEngaged"" ;
VAL_ 648 EPBLunitStatus288 10 ""Disengaging"" 8 ""Engaging"" 3 ""ParkEngaged"" 1 ""DriveReleased"" ;
VAL_ 2047 GTW_activeHighBeam 1 ""ACTIVE"" 0 ""NOT_ACTIVE"" ;
VAL_ 2047 GTW_airSuspension 0 ""NONE"" 2 ""TESLA_ADAPTIVE"" 1 ""TESLA_STANDARD"" ;
VAL_ 2047 GTW_airbagCutoffSwitch 0 ""CUTOFF_SWITCH_DISABLED"" 1 ""CUTOFF_SWITCH_ENABLED"" ;
VAL_ 2047 GTW_audioType 0 ""BASE"" 2 ""BASE_WITH_PREMIUM200"" 1 ""PREMIUM"" ;
VAL_ 2047 GTW_autopilot 4 ""BASIC"" 2 ""ENHANCED"" 1 ""HIGHWAY"" 0 ""NONE"" 3 ""SELF_DRIVING"" ;
VAL_ 2047 GTW_autopilotCameraType 0 ""RCCB_CAMERAS"" ;
VAL_ 2047 GTW_auxParkLamps 2 ""EU"" 0 ""NA_BASE"" 1 ""NA_PREMIUM"" 3 ""NONE"" ;
VAL_ 2047 GTW_bPillarNFCParam 0 ""MODEL_3"" 1 ""MODEL_Y"" ;
VAL_ 2047 GTW_brakeHWType 1 ""BREMBO_LARGE_P42_BREMBO_44MOC"" 3 ""BREMBO_LARGE_P42_BREMBO_LARGE_44MOC"" 2 ""BREMBO_LARGE_P42_MANDO_43MOC"" 0 ""BREMBO_P42_MANDO_43MOC"" ;
VAL_ 2047 GTW_brakeLineSwitchType 0 ""DI_VC_SHARED"" 1 ""VC_ONLY"" ;
VAL_ 2047 GTW_cabinPTCHeaterType 0 ""BORGWARNER"" 1 ""NONE"" ;
VAL_ 2047 GTW_chassisType 2 ""MODEL_3_CHASSIS"" 0 ""MODEL_S_CHASSIS"" 1 ""MODEL_X_CHASSIS"" 3 ""MODEL_Y_CHASSIS"" ;
VAL_ 2047 GTW_compressorType 2 ""DENSO_41CC_11K"" 1 ""DENSO_41CC_8K"" 0 ""HANON_33CC"" ;
VAL_ 2047 GTW_connectivityPackage 0 ""BASE"" 1 ""PREMIUM"" ;
VAL_ 2047 GTW_coolantPumpType 0 ""DUAL"" 1 ""SINGLE_PUMP_BATT"" ;
VAL_ 2047 GTW_dasHw 3 ""PARKER_PASCAL_2_5"" 4 ""TESLA_AP3"" ;
VAL_ 2047 GTW_deliveryStatus 1 ""DELIVERED"" 0 ""NOT_DELIVERED"" ;
VAL_ 2047 GTW_drivetrainType 1 ""AWD"" 0 ""RWD"" ;
VAL_ 2047 GTW_eBuckConfig 1 ""DEV_BUCK"" 0 ""NONE"" ;
VAL_ 2047 GTW_eCallEnabled 0 ""DISABLED"" 1 ""ENABLED_OHC_SOS"" 2 ""ENABLED_UI_SOS"" ;
VAL_ 2047 GTW_efficiencyPackage 0 ""DEFAULT"" 2 ""M3_LR_2020"" 3 ""M3_LR_PERFORMANCE_2020"" 1 ""M3_SR_PLUS_2020"" ;
VAL_ 2047 GTW_epasType 0 ""MANDO_VGR69_GEN3"" ;
VAL_ 2047 GTW_espValveType 0 ""UNKNOWN"" 1 ""VALVE_TYPE_1"" 2 ""VALVE_TYPE_2"" ;
VAL_ 2047 GTW_exteriorColor 5 ""DEEP_BLUE"" 3 ""MIDNIGHT_SILVER"" 6 ""PEARL_WHITE"" 0 ""RED_MULTICOAT"" 2 ""SILVER_METALLIC"" 1 ""SOLID_BLACK"" ;
VAL_ 2047 GTW_frontFogLamps 1 ""INSTALLED"" 0 ""NOT_INSTALLED"" ;
VAL_ 2047 GTW_frontSeatHeaters 1 ""KONGSBERG_LOW_POWER"" 0 ""NONE"" ;
VAL_ 2047 GTW_frontSeatReclinerHardware 3 ""LEFT_RIGHT_SEAT_REDUCED_RANGE"" 2 ""LEFT_SEAT_REDUCED_RANGE"" 1 ""RIGHT_SEAT_REDUCED_RANGE"" 0 ""STANDARD_RANGE"" ;
VAL_ 2047 GTW_frontSeatType 0 ""BASE_TESLA"" 3 ""PREMIUM_L_TESLA_R_YANFENG"" 2 ""PREMIUM_L_YANFENG_R_TESLA"" 1 ""PREMIUM_TESLA"" 4 ""PREMIUM_YANFENG"" ;
VAL_ 2047 GTW_headlamps 0 ""BASE"" 1 ""PREMIUM"" ;
VAL_ 2047 GTW_headlightLevelerType 1 ""GEN1"" 0 ""NONE"" ;
VAL_ 2047 GTW_homelinkType 1 ""HOMELINK_V_OPT_2"" 0 ""NONE"" ;
VAL_ 2047 GTW_hvacPanelVaneType 1 ""CONVERGENT_V1"" 0 ""PARALLEL_V1"" ;
VAL_ 2047 GTW_immersiveAudio 1 ""BASE"" 0 ""DISABLED"" 2 ""PREMIUM"" ;
VAL_ 2047 GTW_interiorLighting 0 ""BASE"" 1 ""PREMIUM"" 2 ""PREMIUM_NO_POCKET_LIGHT"" ;
VAL_ 2047 GTW_intrusionSensorType 0 ""NOT_INSTALLED"" 1 ""VODAFONE"" ;
VAL_ 2047 GTW_lumbarECUType 1 ""ALFMEIER"" 0 ""NONE"" ;
VAL_ 2047 GTW_mapRegion 4 ""AU"" 3 ""CN"" 1 ""EU"" 9 ""HK"" 5 ""JP"" 7 ""KR"" 8 ""ME"" 10 ""MO"" 2 ""NONE"" 6 ""TW"" 0 ""US"" ;
VAL_ 2047 GTW_memoryMirrors 0 ""NOT_INSTALLED"" 1 ""SMR"" ;
VAL_ 2047 GTW_numberHVILNodes 0 ""HVIL_NODES_0"" 1 ""HVIL_NODES_1"" 2 ""HVIL_NODES_2"" 3 ""HVIL_NODES_3"" 4 ""HVIL_NODES_4"" 5 ""HVIL_NODES_5"" ;
VAL_ 2047 GTW_packEnergy 3 ""PACK_100_KWH"" 0 ""PACK_50_KWH"" 2 ""PACK_62_KWH"" 1 ""PACK_74_KWH"" 4 ""PACK_75_KWH"" ;
VAL_ 2047 GTW_passengerAirbagType 2 ""EUROW"" 0 ""FULL_SUPPRESSION"" 1 ""SAFETY_VENT"" ;
VAL_ 2047 GTW_passengerOccupancySensorType 0 ""OCS"" 1 ""RESISTIVE_PAD"" ;
VAL_ 2047 GTW_pedestrianWarningSound 0 ""NONE"" 1 ""SPEAKER"" ;
VAL_ 2047 GTW_performancePackage 0 ""BASE"" 3 ""BASE_PLUS"" 4 ""BASE_PLUS_AWD"" 2 ""LUDICROUS"" 1 ""PERFORMANCE"" ;
VAL_ 2047 GTW_plcSupportType 2 ""NATIVE_CHARGE_PORT"" 0 ""NONE"" 1 ""ONBOARD_ADAPTER"" ;
VAL_ 2047 GTW_powerSteeringColumn 0 ""NOT_INSTALLED"" 1 ""TK"" ;
VAL_ 2047 GTW_radarHeaterType 1 ""BECKER_THIN_3M"" 0 ""NONE"" ;
VAL_ 2047 GTW_rearFogLamps 1 ""INSTALLED"" 0 ""NOT_INSTALLED"" ;
VAL_ 2047 GTW_rearGlassType 0 ""NX"" 1 ""TSA5_NOPET"" ;
VAL_ 2047 GTW_rearLightType 1 ""EU_CN"" 2 ""GLOBAL"" 0 ""NA"" ;
VAL_ 2047 GTW_rearSeatHeaters 1 ""KONGSBERG_LOW_POWER"" 0 ""NONE"" ;
VAL_ 2047 GTW_refrigerantType 0 ""Default"" 2 ""R1234YF"" 1 ""R134A"" ;
VAL_ 2047 GTW_restraintsHardwareType 22 ""EUROW_ECALL_M3"" 33 ""EUROW_ECALL_MY"" 23 ""EUROW_NO_ECALL_M3"" 34 ""EUROW_NO_ECALL_MY"" 21 ""NA_M3"" 32 ""NA_MY"" 31 ""NA_MY_OLD"" ;
VAL_ 2047 GTW_rightHandDrive 0 ""LEFT"" 1 ""RIGHT"" ;
VAL_ 2047 GTW_roofGlassType 0 ""TSA3_PET"" 1 ""TSA5_NOPET"" ;
VAL_ 2047 GTW_roofType 1 ""FIXED_GLASS"" 0 ""METAL"" 2 ""PANORAMIC"" ;
VAL_ 2047 GTW_softRange 1 ""RANGE_220_MILES"" 2 ""RANGE_93_MILES"" 0 ""STANDARD"" ;
VAL_ 2047 GTW_spoilerType 0 ""NOT_INSTALLED"" 1 ""PASSIVE"" ;
VAL_ 2047 GTW_steeringColumnMotorType 0 ""BOSCH"" 1 ""JE"" ;
VAL_ 2047 GTW_steeringColumnUJointType 0 ""B_SAMPLE_PHASING"" 1 ""C_SAMPLE_PHASING"" ;
VAL_ 2047 GTW_superchargingAccess 1 ""ALLOWED"" 0 ""NOT_ALLOWED"" 2 ""PAY_AS_YOU_GO"" ;
VAL_ 2047 GTW_tireType 4 ""CONTI_ALL_SEASON_19"" 17 ""GOODYEAR_ALL_SEASON_20"" 3 ""HANKOOK_SUMMER_19"" 1 ""MICHELIN_ALL_SEASON_18"" 19 ""MICHELIN_ALL_SEASON_21"" 2 ""MICHELIN_SUMMER_18"" 5 ""MICHELIN_SUMMER_20"" 18 ""PIRELLI_SUMMER_21"" 0 ""UNKNOWN"" ;
VAL_ 2047 GTW_towPackage 0 ""NONE"" 1 ""TESLA_REV1"" ;
VAL_ 2047 GTW_tpmsType 0 ""CONTI_2"" 1 ""TESLA_BLE"" ;
VAL_ 2047 GTW_twelveVBatteryType 0 ""ATLASBX_B24_FLOODED"" 1 ""CLARIOS_B24_FLOODED"" ;
VAL_ 2047 GTW_vdcType 0 ""BOSCH_VDC"" 1 ""TESLA_VDC"" ;
VAL_ 2047 GTW_wheelType 4 ""GEMINI_19_SQUARE"" 5 ""GEMINI_19_STAGGERED"" 0 ""PINWHEEL_18"" 18 ""PINWHEEL_18_CAP_KIT"" 1 ""STILETTO_19"" 2 ""STILETTO_20"" 14 ""STILETTO_20_DARK_SQUARE"" 3 ""STILETTO_20_DARK_STAGGERED"" 19 ""ZEROG_20_GUNPOWDER"" 17 ""APOLLO_19_SILVER"" 20 ""APOLLO_19_SILVER_CAP_KIT"" 15 ""INDUCTION_20_BLACK"" 16 ""UBERTURBINE_21_BLACK"" ;
VAL_ 2047 GTW_windshieldType 1 ""EASTMAN_ACOUSTIC"" 0 ""SEKISUI_ACOUSTIC"" ;
VAL_ 2047 GTW_xcpESP 0 ""FALSE"" 1 ""TRUE"" ;
VAL_ 2047 GTW_xcpIbst 0 ""FALSE"" 1 ""TRUE"" ;
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