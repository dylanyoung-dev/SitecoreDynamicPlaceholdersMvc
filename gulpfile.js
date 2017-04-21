var gulp = require("gulp");
var msbuild = require("gulp-msbuild");
var debug = require("gulp-debug");
var path = require("path");
var nugetRestore = require("gulp-nuget-restore");
var fs = require("fs");
var util = require("gulp-util");

var merge = require("merge-stream");
var runSequence = require("run-sequence");

var config = require('./gulp-config.js')();

module.exports.config = config;

gulp.task('default', function (callback) {
    return runSequence(
        "Copy-Sitecore-Dlls",
        callback);
});

gulp.task('Copy-Sitecore-Dlls', function () {

    fs.statSync(config.sitecoreLibraries);

    var files = config.sitecoreLibraries + "/**/*";

    var libs = gulp.src(files).pipe(gulp.dest("./Libraries"));

    return merge(libs);

});