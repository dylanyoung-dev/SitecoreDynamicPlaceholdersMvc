module.exports = function () {
    var sitecoreRoot = "C:\\Sitecore\\Example-82";
    var config = {
        websiteRoot: sitecoreRoot + "\\Website",
        sitecoreLibraries: sitecoreRoot + "\\Website\\bin",
        solutionName: "Example-82",
        licensePath: sitecoreRoot + "\\Data\\license.xml",
        runCleanBuilds: false
    };
    return config;
}