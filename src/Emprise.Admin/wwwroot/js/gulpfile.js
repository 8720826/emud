'use strict';

var gulp = require('gulp');
var $ = require('gulp-load-plugins')();
var browserify = require('browserify');
var transform = require('vinyl-transform');
var markJSON = require('markit-json');
var docUtil = require('amazeui-doc-util');
var browserSync = require('browser-sync');
var del = require('del');
var runSequence = require('run-sequence');
var reload = browserSync.reload;

gulp.task('clean', function(cb) {
  del('dist', cb);
});

gulp.task('uglify', function() {
  return gulp.src('amazeui.switch.js')
    .pipe($.uglify())
    .pipe($.rename(function(file) {
      file.basename = file.basename.toLowerCase();
      if (file.basename === 'readme') {
        file.basename = 'index';
      }
      file.extname = '.min.js';
    }))
    .pipe(gulp.dest('./'));
});

gulp.task('copy', function() {
  return gulp.src('amazeui.switch*.js')
    .pipe(gulp.dest('dist'));
});

gulp.task('docs', function(){
  return gulp.src(['README.md', 'docs/*.md'])
    .pipe(markJSON(docUtil.markedOptions))
    .pipe(docUtil.applyTemplate(null, {
      pluginTitle: 'Amaze UI Switch',
      pluginDesc: '使用 Amaze UI 样式风格的 jQuery Switch 插件。',
      buttons: 'amazeui/switch',
      head: '<link rel="stylesheet" href="../amazeui.switch.css"/>'
    }))
    .pipe($.rename(function(file) {
      file.basename = file.basename.toLowerCase();
      if (file.basename === 'readme') {
        file.basename = 'index';
      }
      file.extname = '.html';
    }))
    .pipe(gulp.dest(function(file) {
      if (file.relative === 'index.html') {
        return 'dist'
      }
      return 'dist/docs';
    }));
});

gulp.task('less', function() {
  return gulp.src('src/amazeui.switch.less')
    .pipe($.less())
    .pipe($.autoprefixer({browsers: docUtil.autoprefixerBrowsers}))
    .pipe($.csso())
    .pipe(gulp.dest('./dist'))
    .pipe(gulp.dest('./'));
});

gulp.task('bundle', function() {
  var bundler = transform(function(filename) {
    var b = browserify({
      entries: filename,
      basedir: './'
    });
    return b.bundle();
  });

  gulp.src('test/main.js')
    .pipe(bundler)
    .pipe($.rename({
      basename: 'bundle'
    }))
    .pipe(gulp.dest('test'))
});

// Watch Files For Changes & Reload
gulp.task('serve', ['default'], function () {
  browserSync({
    notify: false,
    server: 'dist',
    logPrefix: 'AMP'
  }, function(err, bs) {
    console.log(bs.options.getIn(['urls', 'local']));
  });

  gulp.watch('dist/**/*', reload);
});

gulp.task('deploy', ['default'], function() {
  return gulp.src('dist/**/*')
    .pipe($.ghPages());
});

gulp.task('watch', function() {
  gulp.watch(['README.md', 'docs/*.md'], ['docs']);
  gulp.watch('src/*.less', ['less']);
});

gulp.task('default', function(cb) {
  runSequence('clean', ['copy', 'less', 'docs', 'watch'], cb);
});
