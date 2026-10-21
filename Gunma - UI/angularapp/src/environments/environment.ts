// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,

  //FWG Server
  CompanyCode: 'GMA',
  CompanyName: 'GUNMA GUARDFORCE DS SDN BHD.',
  Registration: '(496624-D)',
  CompanyEPF: '12968205',
  CompanyPIC: 'Madam Punitha',
  CompanyPICContact: '0129022222',

  // Bank configuration
  CIMB: 'CIMB',
  CIMB_OrganizationCode: '69778',
  CIMB_OrganizationName: 'FREIGHTWATCH G PATOMOTOH SECURITY SERVIC',
  CIMB_SecurityCode: '0000000000000000',
  CIMB_BNMCode: '3500000',

  CompanyRegNumber: '496624-D',
  SocsoCompanyCode: 'E2100012138Y',

  PayTypeEPF: "EPF",
  PayTypeSOCSO: "Socso",
  PayTypeSIP: "SIP",

  BSN: 'BSN',
  BSN_OrganizationCode: 'F2044800',
  BSN_OrganizationName: 'GUNMA GUARDFORCE DS SDN BHD',
  BSN_SecurityCode: '0000000000000000',
  BSN_BNMCode: '3500000',

  //Local IIS Serverconfiguration
  //IIS Server configuration
  baseUrl: 'http://localhost:5003/api/',
  baseReportUrl: 'http://localhost:8080/',
};

export const TAX = {
  GSTStart6: "2011/01/01",
  GSTEnd6: "2018/05/31",
  GSTStart0: "2018/06/01",
  GSTEnd0: "2018/08/31",
  SSTStart6: "2018/09/01",
  SSTEnd6: "9999/12/31",
  SSTStart8: "2024/03/01",
  SSTEnd8: "9999/12/31",
}

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
