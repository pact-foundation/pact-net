





<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="utf-8">
  <link rel="dns-prefetch" href="https://assets-cdn.github.com">
  <link rel="dns-prefetch" href="https://avatars0.githubusercontent.com">
  <link rel="dns-prefetch" href="https://avatars1.githubusercontent.com">
  <link rel="dns-prefetch" href="https://avatars2.githubusercontent.com">
  <link rel="dns-prefetch" href="https://avatars3.githubusercontent.com">
  <link rel="dns-prefetch" href="https://github-cloud.s3.amazonaws.com">
  <link rel="dns-prefetch" href="https://user-images.githubusercontent.com/">



  <link crossorigin="anonymous" href="https://assets-cdn.github.com/assets/frameworks-e04a23d39bf81b7db3c635177756ef51bc171feb440be9e174933c6eb56382da.css" integrity="sha256-4Eoj05v4G32zxjUXd1bvUbwXH+tEC+nhdJM8brVjgto=" media="all" rel="stylesheet" />
  <link crossorigin="anonymous" href="https://assets-cdn.github.com/assets/github-3dc65358913934c77ed38ef6979525393a285062908a13a901527639c133c821.css" integrity="sha256-PcZTWJE5NMd+0472l5UlOTooUGKQihOpAVJ2OcEzyCE=" media="all" rel="stylesheet" />
  
  
  
  

  <meta name="viewport" content="width=device-width">
  
  <title>pact-net/README.md at master · SEEK-Jobs/pact-net</title>
  <link rel="search" type="application/opensearchdescription+xml" href="/opensearch.xml" title="GitHub">
  <link rel="fluid-icon" href="https://github.com/fluidicon.png" title="GitHub">
  <meta property="fb:app_id" content="1401488693436528">

    
    <meta content="https://avatars0.githubusercontent.com/u/1829110?v=3&amp;s=400" property="og:image" /><meta content="GitHub" property="og:site_name" /><meta content="object" property="og:type" /><meta content="SEEK-Jobs/pact-net" property="og:title" /><meta content="https://github.com/SEEK-Jobs/pact-net" property="og:url" /><meta content="pact-net - .NET version of Pact. Enables consumer driven contract testing, providing a mock service and DSL for the consumer project, and interaction playback and verification for the service provi..." property="og:description" />

  <link rel="assets" href="https://assets-cdn.github.com/">
  <link rel="web-socket" href="wss://live.github.com/_sockets/VjI6MTc0MDAwMDI2OmY1MjAxNTg3ZGU4ZWI0OTlmYmQ2OWI4ODYzM2Q0MjFlYmZlYTczNzcwZmRhMzA0NDU4M2UwMTZjNTkxNzQxOTY=--83b53d93f4a851246a1b3e9004a1b9e6ae41e7a3">
  <meta name="pjax-timeout" content="1000">
  <link rel="sudo-modal" href="/sessions/sudo_modal">
  <meta name="request-id" content="D1CD:1CC24:E00558:164DA2B:59589BAC" data-pjax-transient>
  

  <meta name="selected-link" value="repo_source" data-pjax-transient>

  <meta name="google-site-verification" content="KT5gs8h0wvaagLKAVWq8bbeNwnZZK1r1XQysX3xurLU">
<meta name="google-site-verification" content="ZzhVyEFwb7w3e0-uOTltm8Jsck2F5StVihD0exw2fsA">
    <meta name="google-analytics" content="UA-3769691-2">

<meta content="collector.githubapp.com" name="octolytics-host" /><meta content="github" name="octolytics-app-id" /><meta content="https://collector.githubapp.com/github-external/browser_event" name="octolytics-event-url" /><meta content="D1CD:1CC24:E00558:164DA2B:59589BAC" name="octolytics-dimension-request_id" /><meta content="sea" name="octolytics-dimension-region_edge" /><meta content="iad" name="octolytics-dimension-region_render" /><meta content="1495003" name="octolytics-actor-id" /><meta content="neilcampbell" name="octolytics-actor-login" /><meta content="38c49fc6b565b76f3e82d4e32d96d16296e88eb245f1e5ff3ddd37888a09e4c1" name="octolytics-actor-hash" />
<meta content="/&lt;user-name&gt;/&lt;repo-name&gt;/blob/show" data-pjax-transient="true" name="analytics-location" />




  <meta class="js-ga-set" name="dimension1" content="Logged In">


  

      <meta name="hostname" content="github.com">
  <meta name="user-login" content="neilcampbell">

      <meta name="expected-hostname" content="github.com">
    <meta name="js-proxy-site-detection-payload" content="ODUyM2JlNDA1Nzc3YWI4ODM5MDMzZDdhNjc1MDZlMmYyYmIxNjljZjU4NzAxMjVmMzBlNmYwZmVmODE0YjZlMXx7InJlbW90ZV9hZGRyZXNzIjoiMjEwLjg0LjU5LjE4IiwicmVxdWVzdF9pZCI6IkQxQ0Q6MUNDMjQ6RTAwNTU4OjE2NERBMkI6NTk1ODlCQUMiLCJ0aW1lc3RhbXAiOjE0OTg5NzkyNTAsImhvc3QiOiJnaXRodWIuY29tIn0=">


  <meta name="html-safe-nonce" content="57311724de2f8b2cecee14c1adb82f916fd5f231">

  <meta http-equiv="x-pjax-version" content="e87457103958760f131cc966530f53fd">
  

      <link href="https://github.com/SEEK-Jobs/pact-net/commits/master.atom" rel="alternate" title="Recent Commits to pact-net:master" type="application/atom+xml">

  <meta name="description" content="pact-net - .NET version of Pact. Enables consumer driven contract testing, providing a mock service and DSL for the consumer project, and interaction playback and verification for the service provider project.">
  <meta name="go-import" content="github.com/SEEK-Jobs/pact-net git https://github.com/SEEK-Jobs/pact-net.git">

  <meta content="1829110" name="octolytics-dimension-user_id" /><meta content="SEEK-Jobs" name="octolytics-dimension-user_login" /><meta content="21374320" name="octolytics-dimension-repository_id" /><meta content="SEEK-Jobs/pact-net" name="octolytics-dimension-repository_nwo" /><meta content="true" name="octolytics-dimension-repository_public" /><meta content="false" name="octolytics-dimension-repository_is_fork" /><meta content="21374320" name="octolytics-dimension-repository_network_root_id" /><meta content="SEEK-Jobs/pact-net" name="octolytics-dimension-repository_network_root_nwo" /><meta content="false" name="octolytics-dimension-repository_explore_github_marketplace_ci_cta_shown" />


    <link rel="canonical" href="https://github.com/SEEK-Jobs/pact-net/blob/master/README.md" data-pjax-transient>


  <meta name="browser-stats-url" content="https://api.github.com/_private/browser/stats">

  <meta name="browser-errors-url" content="https://api.github.com/_private/browser/errors">

  <link rel="mask-icon" href="https://assets-cdn.github.com/pinned-octocat.svg" color="#000000">
  <link rel="icon" type="image/x-icon" href="https://assets-cdn.github.com/favicon.ico">

<meta name="theme-color" content="#1e2327">


  <meta name="u2f-support" content="true">

  </head>

  <body class="logged-in env-production page-blob">
    



  <div class="position-relative js-header-wrapper ">
    <a href="#start-of-content" tabindex="1" class="bg-black text-white p-3 show-on-focus js-skip-to-content">Skip to content</a>
    <div id="js-pjax-loader-bar" class="pjax-loader-bar"><div class="progress"></div></div>

    
    
    



        
<div class="header" role="banner">
  <div class="container clearfix">
    <a class="header-logo-invertocat" href="https://github.com/" data-hotkey="g d" aria-label="Homepage" data-ga-click="Header, go to dashboard, icon:logo">
  <svg aria-hidden="true" class="octicon octicon-mark-github" height="32" version="1.1" viewBox="0 0 16 16" width="32"><path fill-rule="evenodd" d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.013 8.013 0 0 0 16 8c0-4.42-3.58-8-8-8z"/></svg>
</a>


        <div class="header-search scoped-search site-scoped-search js-site-search" role="search">
  <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/SEEK-Jobs/pact-net/search" class="js-site-search-form" data-scoped-search-url="/SEEK-Jobs/pact-net/search" data-unscoped-search-url="/search" method="get"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /></div>
    <label class="form-control header-search-wrapper js-chromeless-input-container">
        <a href="/SEEK-Jobs/pact-net/blob/master/README.md" class="header-search-scope no-underline">This repository</a>
      <input type="text"
        class="form-control header-search-input js-site-search-focus js-site-search-field is-clearable"
        data-hotkey="s"
        name="q"
        value=""
        placeholder="Search"
        aria-label="Search this repository"
        data-unscoped-placeholder="Search GitHub"
        data-scoped-placeholder="Search"
        autocapitalize="off">
        <input type="hidden" class="js-site-search-type-field" name="type" >
    </label>
</form></div>


      <ul class="header-nav float-left" role="navigation">
        <li class="header-nav-item">
          <a href="/pulls" aria-label="Pull requests you created" class="js-selected-navigation-item header-nav-link" data-ga-click="Header, click, Nav menu - item:pulls context:user" data-hotkey="g p" data-selected-links="/pulls /pulls/assigned /pulls/mentioned /pulls">
            Pull requests
</a>        </li>
        <li class="header-nav-item">
          <a href="/issues" aria-label="Issues you created" class="js-selected-navigation-item header-nav-link" data-ga-click="Header, click, Nav menu - item:issues context:user" data-hotkey="g i" data-selected-links="/issues /issues/assigned /issues/mentioned /issues">
            Issues
</a>        </li>
            <li class="header-nav-item">
              <a href="/marketplace" class="js-selected-navigation-item header-nav-link" data-ga-click="Header, click, Nav menu - item:marketplace context:user" data-selected-links=" /marketplace">
                Marketplace
</a>            </li>
          <li class="header-nav-item">
            <a class="header-nav-link" href="https://gist.github.com/" data-ga-click="Header, go to gist, text:gist">Gist</a>
          </li>
      </ul>

    
<ul class="header-nav user-nav float-right" id="user-links">
  <li class="header-nav-item">
    
    <a href="/notifications" aria-label="You have unread notifications" class="header-nav-link notification-indicator tooltipped tooltipped-s js-socket-channel js-notification-indicator " data-channel="notification-changed:1495003" data-ga-click="Header, go to notifications, icon:unread" data-hotkey="g n">
        <span class="mail-status unread"></span>
        <svg aria-hidden="true" class="octicon octicon-bell float-left" height="16" version="1.1" viewBox="0 0 14 16" width="14"><path fill-rule="evenodd" d="M14 12v1H0v-1l.73-.58c.77-.77.81-2.55 1.19-4.42C2.69 3.23 6 2 6 2c0-.55.45-1 1-1s1 .45 1 1c0 0 3.39 1.23 4.16 5 .38 1.88.42 3.66 1.19 4.42l.66.58H14zm-7 4c1.11 0 2-.89 2-2H5c0 1.11.89 2 2 2z"/></svg>
</a>
  </li>

  <li class="header-nav-item dropdown js-menu-container">
    <a class="header-nav-link tooltipped tooltipped-s js-menu-target" href="/new"
       aria-label="Create new…"
       aria-expanded="false"
       aria-haspopup="true"
       data-ga-click="Header, create new, icon:add">
      <svg aria-hidden="true" class="octicon octicon-plus float-left" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 9H7v5H5V9H0V7h5V2h2v5h5z"/></svg>
      <span class="dropdown-caret"></span>
    </a>

    <div class="dropdown-menu-content js-menu-content">
      <ul class="dropdown-menu dropdown-menu-sw">
        
<a class="dropdown-item" href="/new" data-ga-click="Header, create new repository">
  New repository
</a>

  <a class="dropdown-item" href="/new/import" data-ga-click="Header, import a repository">
    Import repository
  </a>

<a class="dropdown-item" href="https://gist.github.com/" data-ga-click="Header, create new gist">
  New gist
</a>

  <a class="dropdown-item" href="/organizations/new" data-ga-click="Header, create new organization">
    New organization
  </a>



  <div class="dropdown-divider"></div>
  <div class="dropdown-header">
    <span title="SEEK-Jobs/pact-net">This repository</span>
  </div>
    <a class="dropdown-item" href="/SEEK-Jobs/pact-net/issues/new" data-ga-click="Header, create new issue">
      New issue
    </a>

      </ul>
    </div>
  </li>

  <li class="header-nav-item dropdown js-menu-container">
    <a class="header-nav-link name tooltipped tooltipped-sw js-menu-target" href="/neilcampbell"
       aria-label="View profile and more"
       aria-expanded="false"
       aria-haspopup="true"
       data-ga-click="Header, show menu, icon:avatar">
      <img alt="@neilcampbell" class="avatar" src="https://avatars1.githubusercontent.com/u/1495003?v=3&amp;s=40" height="20" width="20">
      <span class="dropdown-caret"></span>
    </a>

    <div class="dropdown-menu-content js-menu-content">
      <div class="dropdown-menu dropdown-menu-sw">
        <div class="dropdown-header header-nav-current-user css-truncate">
          Signed in as <strong class="css-truncate-target">neilcampbell</strong>
        </div>

        <div class="dropdown-divider"></div>

        <a class="dropdown-item" href="/neilcampbell" data-ga-click="Header, go to profile, text:your profile">
          Your profile
        </a>
        <a class="dropdown-item" href="/neilcampbell?tab=stars" data-ga-click="Header, go to starred repos, text:your stars">
          Your stars
        </a>
        <a class="dropdown-item" href="/explore" data-ga-click="Header, go to explore, text:explore">
          Explore
        </a>
        <a class="dropdown-item" href="https://help.github.com" data-ga-click="Header, go to help, text:help">
          Help
        </a>

        <div class="dropdown-divider"></div>

        <a class="dropdown-item" href="/settings/profile" data-ga-click="Header, go to settings, icon:settings">
          Settings
        </a>

        <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/logout" class="logout-form" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="auk9/qzxsXZCOle5AwbYAm95rCXZszQk2jPWXNvx8iGn2K9MQ1lJVpvU0QLrpeeuX73NiWTmZvfo0A1KulpFEw==" /></div>
          <button type="submit" class="dropdown-item dropdown-signout" data-ga-click="Header, sign out, icon:logout">
            Sign out
          </button>
</form>      </div>
    </div>
  </li>
</ul>


    <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/logout" class="sr-only right-0" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="59VVQvIf90DAciUUOUXDVG6Zvpe5Bzg9CM3rarIfoXAq5MfwHbcPYBmco6/R5vz4Xl3fOwRSau46LjB807QWQg==" /></div>
      <button type="submit" class="dropdown-item dropdown-signout" data-ga-click="Header, sign out, icon:logout">
        Sign out
      </button>
</form>  </div>
</div>


      

  </div>

  <div id="start-of-content" class="show-on-focus"></div>

    <div id="js-flash-container">
</div>



  <div role="main">
        <div itemscope itemtype="http://schema.org/SoftwareSourceCode">
    <div id="js-repo-pjax-container" data-pjax-container>
      



  


    <div class="pagehead repohead instapaper_ignore readability-menu experiment-repo-nav">
      <div class="container repohead-details-container">

        <ul class="pagehead-actions">
  <li>
        <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/notifications/subscribe" class="js-social-container" data-autosubmit="true" data-remote="true" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="3rNCqUZ5bUYwT1KXugzKggN2gYW8375IhYZGNze5pMmEOSyrg54xNx5wehXlDwW0v0Ll/8gQW1ByQ+dEJPsOYw==" /></div>      <input class="form-control" id="repository_id" name="repository_id" type="hidden" value="21374320" />

        <div class="select-menu js-menu-container js-select-menu">
          <a href="/SEEK-Jobs/pact-net/subscription"
            class="btn btn-sm btn-with-count select-menu-button js-menu-target"
            role="button"
            aria-haspopup="true"
            aria-expanded="false"
            aria-label="Toggle repository notifications menu"
            data-ga-click="Repository, click Watch settings, action:blob#show">
            <span class="js-select-button">
                <svg aria-hidden="true" class="octicon octicon-eye" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M8.06 2C3 2 0 8 0 8s3 6 8.06 6C13 14 16 8 16 8s-3-6-7.94-6zM8 12c-2.2 0-4-1.78-4-4 0-2.2 1.8-4 4-4 2.22 0 4 1.8 4 4 0 2.22-1.78 4-4 4zm2-4c0 1.11-.89 2-2 2-1.11 0-2-.89-2-2 0-1.11.89-2 2-2 1.11 0 2 .89 2 2z"/></svg>
                Unwatch
            </span>
          </a>
            <a class="social-count js-social-count"
              href="/SEEK-Jobs/pact-net/watchers"
              aria-label="39 users are watching this repository">
              39
            </a>

        <div class="select-menu-modal-holder">
          <div class="select-menu-modal subscription-menu-modal js-menu-content">
            <div class="select-menu-header js-navigation-enable" tabindex="-1">
              <svg aria-label="Close" class="octicon octicon-x js-menu-close" height="16" role="img" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M7.48 8l3.75 3.75-1.48 1.48L6 9.48l-3.75 3.75-1.48-1.48L4.52 8 .77 4.25l1.48-1.48L6 6.52l3.75-3.75 1.48 1.48z"/></svg>
              <span class="select-menu-title">Notifications</span>
            </div>

              <div class="select-menu-list js-navigation-container" role="menu">

                <div class="select-menu-item js-navigation-item " role="menuitem" tabindex="0">
                  <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
                  <div class="select-menu-item-text">
                    <input id="do_included" name="do" type="radio" value="included" />
                    <span class="select-menu-item-heading">Not watching</span>
                    <span class="description">Be notified when participating or @mentioned.</span>
                    <span class="js-select-button-text hidden-select-button-text">
                      <svg aria-hidden="true" class="octicon octicon-eye" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M8.06 2C3 2 0 8 0 8s3 6 8.06 6C13 14 16 8 16 8s-3-6-7.94-6zM8 12c-2.2 0-4-1.78-4-4 0-2.2 1.8-4 4-4 2.22 0 4 1.8 4 4 0 2.22-1.78 4-4 4zm2-4c0 1.11-.89 2-2 2-1.11 0-2-.89-2-2 0-1.11.89-2 2-2 1.11 0 2 .89 2 2z"/></svg>
                      Watch
                    </span>
                  </div>
                </div>

                <div class="select-menu-item js-navigation-item selected" role="menuitem" tabindex="0">
                  <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
                  <div class="select-menu-item-text">
                    <input checked="checked" id="do_subscribed" name="do" type="radio" value="subscribed" />
                    <span class="select-menu-item-heading">Watching</span>
                    <span class="description">Be notified of all conversations.</span>
                    <span class="js-select-button-text hidden-select-button-text">
                      <svg aria-hidden="true" class="octicon octicon-eye" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M8.06 2C3 2 0 8 0 8s3 6 8.06 6C13 14 16 8 16 8s-3-6-7.94-6zM8 12c-2.2 0-4-1.78-4-4 0-2.2 1.8-4 4-4 2.22 0 4 1.8 4 4 0 2.22-1.78 4-4 4zm2-4c0 1.11-.89 2-2 2-1.11 0-2-.89-2-2 0-1.11.89-2 2-2 1.11 0 2 .89 2 2z"/></svg>
                        Unwatch
                    </span>
                  </div>
                </div>

                <div class="select-menu-item js-navigation-item " role="menuitem" tabindex="0">
                  <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
                  <div class="select-menu-item-text">
                    <input id="do_ignore" name="do" type="radio" value="ignore" />
                    <span class="select-menu-item-heading">Ignoring</span>
                    <span class="description">Never be notified.</span>
                    <span class="js-select-button-text hidden-select-button-text">
                      <svg aria-hidden="true" class="octicon octicon-mute" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M8 2.81v10.38c0 .67-.81 1-1.28.53L3 10H1c-.55 0-1-.45-1-1V7c0-.55.45-1 1-1h2l3.72-3.72C7.19 1.81 8 2.14 8 2.81zm7.53 3.22l-1.06-1.06-1.97 1.97-1.97-1.97-1.06 1.06L11.44 8 9.47 9.97l1.06 1.06 1.97-1.97 1.97 1.97 1.06-1.06L13.56 8l1.97-1.97z"/></svg>
                        Stop ignoring
                    </span>
                  </div>
                </div>

              </div>

            </div>
          </div>
        </div>
</form>
  </li>

  <li>
    
  <div class="js-toggler-container js-social-container starring-container on">
    <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/SEEK-Jobs/pact-net/unstar" class="starred" data-remote="true" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="/8BcCIy1KTcm2Tq9IZwARbom5L4jmGoMC+EoCMHOZ/E6Qnp2O8ApvuBtQzTOKsaDwKqE5+DDt24atK8MApt70g==" /></div>
      <button
        type="submit"
        class="btn btn-sm btn-with-count js-toggler-target"
        aria-label="Unstar this repository" title="Unstar SEEK-Jobs/pact-net"
        data-ga-click="Repository, click unstar button, action:blob#show; text:Unstar">
        <svg aria-hidden="true" class="octicon octicon-star" height="16" version="1.1" viewBox="0 0 14 16" width="14"><path fill-rule="evenodd" d="M14 6l-4.9-.64L7 1 4.9 5.36 0 6l3.6 3.26L2.67 14 7 11.67 11.33 14l-.93-4.74z"/></svg>
        Unstar
      </button>
        <a class="social-count js-social-count" href="/SEEK-Jobs/pact-net/stargazers"
           aria-label="174 users starred this repository">
          174
        </a>
</form>
    <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/SEEK-Jobs/pact-net/star" class="unstarred" data-remote="true" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="pF5ueR8f4+SLMnJB6xbrXTIFHtbNL3JTLYMOJiuHQ4ZaQA+U99u9tq+C04fBR5TXm0iN9Lgb8/OrlQ0ACoAQxA==" /></div>
      <button
        type="submit"
        class="btn btn-sm btn-with-count js-toggler-target"
        aria-label="Star this repository" title="Star SEEK-Jobs/pact-net"
        data-ga-click="Repository, click star button, action:blob#show; text:Star">
        <svg aria-hidden="true" class="octicon octicon-star" height="16" version="1.1" viewBox="0 0 14 16" width="14"><path fill-rule="evenodd" d="M14 6l-4.9-.64L7 1 4.9 5.36 0 6l3.6 3.26L2.67 14 7 11.67 11.33 14l-.93-4.74z"/></svg>
        Star
      </button>
        <a class="social-count js-social-count" href="/SEEK-Jobs/pact-net/stargazers"
           aria-label="174 users starred this repository">
          174
        </a>
</form>  </div>

  </li>

  <li>
          <a href="#fork-destination-box" class="btn btn-sm btn-with-count"
              title="Fork your own copy of SEEK-Jobs/pact-net to your account"
              aria-label="Fork your own copy of SEEK-Jobs/pact-net to your account"
              rel="facebox"
              data-ga-click="Repository, show fork modal, action:blob#show; text:Fork">
              <svg aria-hidden="true" class="octicon octicon-repo-forked" height="16" version="1.1" viewBox="0 0 10 16" width="10"><path fill-rule="evenodd" d="M8 1a1.993 1.993 0 0 0-1 3.72V6L5 8 3 6V4.72A1.993 1.993 0 0 0 2 1a1.993 1.993 0 0 0-1 3.72V6.5l3 3v1.78A1.993 1.993 0 0 0 5 15a1.993 1.993 0 0 0 1-3.72V9.5l3-3V4.72A1.993 1.993 0 0 0 8 1zM2 4.2C1.34 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zm3 10c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zm3-10c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z"/></svg>
            Fork
          </a>

          <div id="fork-destination-box" style="display: none;">
            <h2 class="facebox-header" data-facebox-id="facebox-header">Where should we fork this repository?</h2>
            <include-fragment src=""
                class="js-fork-select-fragment fork-select-fragment"
                data-url="/SEEK-Jobs/pact-net/fork?fragment=1">
              <img alt="Loading" height="64" src="https://assets-cdn.github.com/images/spinners/octocat-spinner-128.gif" width="64" />
            </include-fragment>
          </div>

    <a href="/SEEK-Jobs/pact-net/network" class="social-count"
       aria-label="83 users forked this repository">
      83
    </a>
  </li>
</ul>

        <h1 class="public ">
  <svg aria-hidden="true" class="octicon octicon-repo" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M4 9H3V8h1v1zm0-3H3v1h1V6zm0-2H3v1h1V4zm0-2H3v1h1V2zm8-1v12c0 .55-.45 1-1 1H6v2l-1.5-1.5L3 16v-2H1c-.55 0-1-.45-1-1V1c0-.55.45-1 1-1h10c.55 0 1 .45 1 1zm-1 10H1v2h2v-1h3v1h5v-2zm0-10H2v9h9V1z"/></svg>
  <span class="author" itemprop="author"><a href="/SEEK-Jobs" class="url fn" rel="author">SEEK-Jobs</a></span><!--
--><span class="path-divider">/</span><!--
--><strong itemprop="name"><a href="/SEEK-Jobs/pact-net" data-pjax="#js-repo-pjax-container">pact-net</a></strong>

</h1>

      </div>
      <div class="container">
        
<nav class="reponav js-repo-nav js-sidenav-container-pjax"
     itemscope
     itemtype="http://schema.org/BreadcrumbList"
     role="navigation"
     data-pjax="#js-repo-pjax-container">

  <span itemscope itemtype="http://schema.org/ListItem" itemprop="itemListElement">
    <a href="/SEEK-Jobs/pact-net" class="js-selected-navigation-item selected reponav-item" data-hotkey="g c" data-selected-links="repo_source repo_downloads repo_commits repo_releases repo_tags repo_branches /SEEK-Jobs/pact-net" itemprop="url">
      <svg aria-hidden="true" class="octicon octicon-code" height="16" version="1.1" viewBox="0 0 14 16" width="14"><path fill-rule="evenodd" d="M9.5 3L8 4.5 11.5 8 8 11.5 9.5 13 14 8 9.5 3zm-5 0L0 8l4.5 5L6 11.5 2.5 8 6 4.5 4.5 3z"/></svg>
      <span itemprop="name">Code</span>
      <meta itemprop="position" content="1">
</a>  </span>

    <span itemscope itemtype="http://schema.org/ListItem" itemprop="itemListElement">
      <a href="/SEEK-Jobs/pact-net/issues" class="js-selected-navigation-item reponav-item" data-hotkey="g i" data-selected-links="repo_issues repo_labels repo_milestones /SEEK-Jobs/pact-net/issues" itemprop="url">
        <svg aria-hidden="true" class="octicon octicon-issue-opened" height="16" version="1.1" viewBox="0 0 14 16" width="14"><path fill-rule="evenodd" d="M7 2.3c3.14 0 5.7 2.56 5.7 5.7s-2.56 5.7-5.7 5.7A5.71 5.71 0 0 1 1.3 8c0-3.14 2.56-5.7 5.7-5.7zM7 1C3.14 1 0 4.14 0 8s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7zm1 3H6v5h2V4zm0 6H6v2h2v-2z"/></svg>
        <span itemprop="name">Issues</span>
        <span class="Counter">5</span>
        <meta itemprop="position" content="2">
</a>    </span>

  <span itemscope itemtype="http://schema.org/ListItem" itemprop="itemListElement">
    <a href="/SEEK-Jobs/pact-net/pulls" class="js-selected-navigation-item reponav-item" data-hotkey="g p" data-selected-links="repo_pulls /SEEK-Jobs/pact-net/pulls" itemprop="url">
      <svg aria-hidden="true" class="octicon octicon-git-pull-request" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M11 11.28V5c-.03-.78-.34-1.47-.94-2.06C9.46 2.35 8.78 2.03 8 2H7V0L4 3l3 3V4h1c.27.02.48.11.69.31.21.2.3.42.31.69v6.28A1.993 1.993 0 0 0 10 15a1.993 1.993 0 0 0 1-3.72zm-1 2.92c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zM4 3c0-1.11-.89-2-2-2a1.993 1.993 0 0 0-1 3.72v6.56A1.993 1.993 0 0 0 2 15a1.993 1.993 0 0 0 1-3.72V4.72c.59-.34 1-.98 1-1.72zm-.8 10c0 .66-.55 1.2-1.2 1.2-.65 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2zM2 4.2C1.34 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z"/></svg>
      <span itemprop="name">Pull requests</span>
      <span class="Counter">1</span>
      <meta itemprop="position" content="3">
</a>  </span>




    <div class="reponav-dropdown js-menu-container">
      <button type="button" class="btn-link reponav-item reponav-dropdown js-menu-target " data-no-toggle aria-expanded="false" aria-haspopup="true">
        Insights
        <svg aria-hidden="true" class="octicon octicon-triangle-down v-align-middle text-gray" height="11" version="1.1" viewBox="0 0 12 16" width="8"><path fill-rule="evenodd" d="M0 5l6 6 6-6z"/></svg>
      </button>
      <div class="dropdown-menu-content js-menu-content">
        <div class="dropdown-menu dropdown-menu-sw">
          <a class="dropdown-item" href="/SEEK-Jobs/pact-net/pulse" data-skip-pjax>
            <svg aria-hidden="true" class="octicon octicon-pulse" height="16" version="1.1" viewBox="0 0 14 16" width="14"><path fill-rule="evenodd" d="M11.5 8L8.8 5.4 6.6 8.5 5.5 1.6 2.38 8H0v2h3.6l.9-1.8.9 5.4L9 8.5l1.6 1.5H14V8z"/></svg>
            Pulse
          </a>
          <a class="dropdown-item" href="/SEEK-Jobs/pact-net/graphs" data-skip-pjax>
            <svg aria-hidden="true" class="octicon octicon-graph" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M16 14v1H0V0h1v14h15zM5 13H3V8h2v5zm4 0H7V3h2v10zm4 0h-2V6h2v7z"/></svg>
            Graphs
          </a>
        </div>
      </div>
    </div>
</nav>

      </div>
    </div>

<div class="container new-discussion-timeline experiment-repo-nav">
  <div class="repository-content">

    
  <a href="/SEEK-Jobs/pact-net/blob/96827672a58b681664ff8aae38359365cb5da152/README.md" class="d-none js-permalink-shortcut" data-hotkey="y">Permalink</a>

  <!-- blob contrib key: blob_contributors:v21:20d6f1977ca69c50a6047a4534688606 -->

  <div class="file-navigation js-zeroclipboard-container">
    
<div class="select-menu branch-select-menu js-menu-container js-select-menu float-left">
  <button class=" btn btn-sm select-menu-button js-menu-target css-truncate" data-hotkey="w"
    
    type="button" aria-label="Switch branches or tags" aria-expanded="false" aria-haspopup="true">
      <i>Branch:</i>
      <span class="js-select-button css-truncate-target">master</span>
  </button>

  <div class="select-menu-modal-holder js-menu-content js-navigation-container" data-pjax>

    <div class="select-menu-modal">
      <div class="select-menu-header">
        <svg aria-label="Close" class="octicon octicon-x js-menu-close" height="16" role="img" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M7.48 8l3.75 3.75-1.48 1.48L6 9.48l-3.75 3.75-1.48-1.48L4.52 8 .77 4.25l1.48-1.48L6 6.52l3.75-3.75 1.48 1.48z"/></svg>
        <span class="select-menu-title">Switch branches/tags</span>
      </div>

      <div class="select-menu-filters">
        <div class="select-menu-text-filter">
          <input type="text" aria-label="Find or create a branch…" id="context-commitish-filter-field" class="form-control js-filterable-field js-navigation-enable" placeholder="Find or create a branch…">
        </div>
        <div class="select-menu-tabs">
          <ul>
            <li class="select-menu-tab">
              <a href="#" data-tab-filter="branches" data-filter-placeholder="Find or create a branch…" class="js-select-menu-tab" role="tab">Branches</a>
            </li>
            <li class="select-menu-tab">
              <a href="#" data-tab-filter="tags" data-filter-placeholder="Find a tag…" class="js-select-menu-tab" role="tab">Tags</a>
            </li>
          </ul>
        </div>
      </div>

      <div class="select-menu-list select-menu-tab-bucket js-select-menu-tab-bucket" data-tab-filter="branches" role="menu">

        <div data-filterable-for="context-commitish-filter-field" data-filterable-type="substring">


            <a class="select-menu-item js-navigation-item js-navigation-open "
               href="/SEEK-Jobs/pact-net/blob/feature/regex-response-matching/README.md"
               data-name="feature/regex-response-matching"
               data-skip-pjax="true"
               rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target js-select-menu-filter-text">
                feature/regex-response-matching
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
               href="/SEEK-Jobs/pact-net/blob/feature/returnpact/README.md"
               data-name="feature/returnpact"
               data-skip-pjax="true"
               rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target js-select-menu-filter-text">
                feature/returnpact
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open selected"
               href="/SEEK-Jobs/pact-net/blob/master/README.md"
               data-name="master"
               data-skip-pjax="true"
               rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target js-select-menu-filter-text">
                master
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
               href="/SEEK-Jobs/pact-net/blob/version-2/README.md"
               data-name="version-2"
               data-skip-pjax="true"
               rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target js-select-menu-filter-text">
                version-2
              </span>
            </a>
        </div>

          <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/SEEK-Jobs/pact-net/branches" class="js-create-branch select-menu-item select-menu-new-item-form js-navigation-item js-new-item-form" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="TsdQn71UqF4rI+bIwSmfPNzvsNBB/ssomWyzODqQOBoyx7tEXrR/7vtLNLDbD9JLZWfOoaE/sKAoFtKgLTJ+SQ==" /></div>
          <svg aria-hidden="true" class="octicon octicon-git-branch select-menu-item-icon" height="16" version="1.1" viewBox="0 0 10 16" width="10"><path fill-rule="evenodd" d="M10 5c0-1.11-.89-2-2-2a1.993 1.993 0 0 0-1 3.72v.3c-.02.52-.23.98-.63 1.38-.4.4-.86.61-1.38.63-.83.02-1.48.16-2 .45V4.72a1.993 1.993 0 0 0-1-3.72C.88 1 0 1.89 0 3a2 2 0 0 0 1 1.72v6.56c-.59.35-1 .99-1 1.72 0 1.11.89 2 2 2 1.11 0 2-.89 2-2 0-.53-.2-1-.53-1.36.09-.06.48-.41.59-.47.25-.11.56-.17.94-.17 1.05-.05 1.95-.45 2.75-1.25S8.95 7.77 9 6.73h-.02C9.59 6.37 10 5.73 10 5zM2 1.8c.66 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2C1.35 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2zm0 12.41c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zm6-8c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z"/></svg>
            <div class="select-menu-item-text">
              <span class="select-menu-item-heading">Create branch: <span class="js-new-item-name"></span></span>
              <span class="description">from ‘master’</span>
            </div>
            <input type="hidden" name="name" id="name" class="js-new-item-value">
            <input type="hidden" name="branch" id="branch" value="master">
            <input type="hidden" name="path" id="path" value="README.md">
</form>
      </div>

      <div class="select-menu-list select-menu-tab-bucket js-select-menu-tab-bucket" data-tab-filter="tags">
        <div data-filterable-for="context-commitish-filter-field" data-filterable-type="substring">


            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.3.1/README.md"
              data-name="1.3.1"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.3.1">
                1.3.1
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.3.0/README.md"
              data-name="1.3.0"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.3.0">
                1.3.0
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.3.0-beta/README.md"
              data-name="1.3.0-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.3.0-beta">
                1.3.0-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.2.0/README.md"
              data-name="1.2.0"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.2.0">
                1.2.0
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.1.4/README.md"
              data-name="1.1.4"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.1.4">
                1.1.4
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.1.3/README.md"
              data-name="1.1.3"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.1.3">
                1.1.3
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.1.2/README.md"
              data-name="1.1.2"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.1.2">
                1.1.2
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.1.1/README.md"
              data-name="1.1.1"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.1.1">
                1.1.1
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.1.0/README.md"
              data-name="1.1.0"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.1.0">
                1.1.0
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.11/README.md"
              data-name="1.0.11"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.11">
                1.0.11
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.10/README.md"
              data-name="1.0.10"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.10">
                1.0.10
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.9/README.md"
              data-name="1.0.9"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.9">
                1.0.9
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.8/README.md"
              data-name="1.0.8"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.8">
                1.0.8
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.7/README.md"
              data-name="1.0.7"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.7">
                1.0.7
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.6/README.md"
              data-name="1.0.6"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.6">
                1.0.6
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.5/README.md"
              data-name="1.0.5"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.5">
                1.0.5
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.4/README.md"
              data-name="1.0.4"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.4">
                1.0.4
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.3/README.md"
              data-name="1.0.3"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.3">
                1.0.3
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.2/README.md"
              data-name="1.0.2"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.2">
                1.0.2
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.1/README.md"
              data-name="1.0.1"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.1">
                1.0.1
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.0/README.md"
              data-name="1.0.0"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.0">
                1.0.0
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/1.0.0-beta/README.md"
              data-name="1.0.0-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="1.0.0-beta">
                1.0.0-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.11-beta/README.md"
              data-name="0.1.11-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.11-beta">
                0.1.11-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.10-beta/README.md"
              data-name="0.1.10-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.10-beta">
                0.1.10-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.9-beta/README.md"
              data-name="0.1.9-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.9-beta">
                0.1.9-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.8-beta/README.md"
              data-name="0.1.8-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.8-beta">
                0.1.8-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.7-beta/README.md"
              data-name="0.1.7-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.7-beta">
                0.1.7-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.6-beta/README.md"
              data-name="0.1.6-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.6-beta">
                0.1.6-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.5-beta/README.md"
              data-name="0.1.5-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.5-beta">
                0.1.5-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.4-beta/README.md"
              data-name="0.1.4-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.4-beta">
                0.1.4-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.3-beta/README.md"
              data-name="0.1.3-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.3-beta">
                0.1.3-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.2-beta/README.md"
              data-name="0.1.2-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.2-beta">
                0.1.2-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.1-beta/README.md"
              data-name="0.1.1-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.1-beta">
                0.1.1-beta
              </span>
            </a>
            <a class="select-menu-item js-navigation-item js-navigation-open "
              href="/SEEK-Jobs/pact-net/tree/0.1.0-beta/README.md"
              data-name="0.1.0-beta"
              data-skip-pjax="true"
              rel="nofollow">
              <svg aria-hidden="true" class="octicon octicon-check select-menu-item-icon" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M12 5l-8 8-4-4 1.5-1.5L4 10l6.5-6.5z"/></svg>
              <span class="select-menu-item-text css-truncate-target" title="0.1.0-beta">
                0.1.0-beta
              </span>
            </a>
        </div>

        <div class="select-menu-no-results">Nothing to show</div>
      </div>

    </div>
  </div>
</div>

    <div class="BtnGroup float-right">
      <a href="/SEEK-Jobs/pact-net/find/master"
            class="js-pjax-capture-input btn btn-sm BtnGroup-item"
            data-pjax
            data-hotkey="t">
        Find file
      </a>
      <button aria-label="Copy file path to clipboard" class="js-zeroclipboard btn btn-sm BtnGroup-item tooltipped tooltipped-s" data-copied-hint="Copied!" type="button">Copy path</button>
    </div>
    <div class="breadcrumb js-zeroclipboard-target">
      <span class="repo-root js-repo-root"><span class="js-path-segment"><a href="/SEEK-Jobs/pact-net"><span>pact-net</span></a></span></span><span class="separator">/</span><strong class="final-path">README.md</strong>
    </div>
  </div>


  
  <div class="commit-tease">
      <span class="float-right">
        <a class="commit-tease-sha" href="/SEEK-Jobs/pact-net/commit/80abb889b3aa76d6f4e7beb8b4cf9ac7cd21a408" data-pjax>
          80abb88
        </a>
        <relative-time datetime="2016-12-09T06:14:36Z">Dec 9, 2016</relative-time>
      </span>
      <div>
        <img alt="@neilcampbell" class="avatar" height="20" src="https://avatars1.githubusercontent.com/u/1495003?v=3&amp;s=40" width="20" />
        <a href="/neilcampbell" class="user-mention" rel="contributor">neilcampbell</a>
          <a href="/SEEK-Jobs/pact-net/commit/80abb889b3aa76d6f4e7beb8b4cf9ac7cd21a408" class="message" data-pjax="true" title="Changing badge to show master status">Changing badge to show master status</a>
      </div>

    <div class="commit-tease-contributors">
      <button type="button" class="btn-link muted-link contributors-toggle" data-facebox="#blob_contributors_box">
        <strong>2</strong>
         contributors
      </button>
          <a class="avatar-link tooltipped tooltipped-s" aria-label="neilcampbell" href="/SEEK-Jobs/pact-net/commits/master/README.md?author=neilcampbell"><img alt="@neilcampbell" class="avatar" height="20" src="https://avatars1.githubusercontent.com/u/1495003?v=3&amp;s=40" width="20" /> </a>
    <a class="avatar-link tooltipped tooltipped-s" aria-label="tbenade" href="/SEEK-Jobs/pact-net/commits/master/README.md?author=tbenade"><img alt="@tbenade" class="avatar" height="20" src="https://avatars0.githubusercontent.com/u/2978994?v=3&amp;s=40" width="20" /> </a>


    </div>

    <div id="blob_contributors_box" style="display:none">
      <h2 class="facebox-header" data-facebox-id="facebox-header">Users who have contributed to this file</h2>
      <ul class="facebox-user-list" data-facebox-id="facebox-description">
          <li class="facebox-user-list-item">
            <img alt="@neilcampbell" height="24" src="https://avatars3.githubusercontent.com/u/1495003?v=3&amp;s=48" width="24" />
            <a href="/neilcampbell">neilcampbell</a>
          </li>
          <li class="facebox-user-list-item">
            <img alt="@tbenade" height="24" src="https://avatars2.githubusercontent.com/u/2978994?v=3&amp;s=48" width="24" />
            <a href="/tbenade">tbenade</a>
          </li>
      </ul>
    </div>
  </div>

  <div class="file">
    <div class="file-header">
  <div class="file-actions">

    <div class="BtnGroup">
      <a href="/SEEK-Jobs/pact-net/raw/master/README.md" class="btn btn-sm BtnGroup-item" id="raw-url">Raw</a>
        <a href="/SEEK-Jobs/pact-net/blame/master/README.md" class="btn btn-sm js-update-url-with-hash BtnGroup-item" data-hotkey="b">Blame</a>
      <a href="/SEEK-Jobs/pact-net/commits/master/README.md" class="btn btn-sm BtnGroup-item" rel="nofollow">History</a>
    </div>

        <a class="btn-octicon tooltipped tooltipped-nw"
           href="github-mac://openRepo/https://github.com/SEEK-Jobs/pact-net?branch=master&amp;filepath=README.md"
           aria-label="Open this file in GitHub Desktop"
           data-ga-click="Repository, open with desktop, type:windows">
            <svg aria-hidden="true" class="octicon octicon-device-desktop" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M15 2H1c-.55 0-1 .45-1 1v9c0 .55.45 1 1 1h5.34c-.25.61-.86 1.39-2.34 2h8c-1.48-.61-2.09-1.39-2.34-2H15c.55 0 1-.45 1-1V3c0-.55-.45-1-1-1zm0 9H1V3h14v8z"/></svg>
        </a>

        <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/SEEK-Jobs/pact-net/edit/master/README.md" class="inline-form js-update-url-with-hash" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="EUUNPU4Pmy7xEBwaI3XSHnOhUZryr0UYLJyyumYVw8RP42H7raA+VStPFIy6BrsmHByrwT89hKUQqOX+8+ZuKA==" /></div>
          <button class="btn-octicon tooltipped tooltipped-nw" type="submit"
            aria-label="Edit this file" data-hotkey="e" data-disable-with>
            <svg aria-hidden="true" class="octicon octicon-pencil" height="16" version="1.1" viewBox="0 0 14 16" width="14"><path fill-rule="evenodd" d="M0 12v3h3l8-8-3-3-8 8zm3 2H1v-2h1v1h1v1zm10.3-9.3L12 6 9 3l1.3-1.3a.996.996 0 0 1 1.41 0l1.59 1.59c.39.39.39 1.02 0 1.41z"/></svg>
          </button>
</form>        <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="/SEEK-Jobs/pact-net/delete/master/README.md" class="inline-form" method="post"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /><input name="authenticity_token" type="hidden" value="7ScSUXVsq6taLY7pwckmx9W+MbOIZu20vaoUFt0eOy4VCSopIzJ82tJOy098p4BB7kO0cXRejz/C+e0qVwRhbA==" /></div>
          <button class="btn-octicon btn-octicon-danger tooltipped tooltipped-nw" type="submit"
            aria-label="Delete this file" data-disable-with>
            <svg aria-hidden="true" class="octicon octicon-trashcan" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M11 2H9c0-.55-.45-1-1-1H5c-.55 0-1 .45-1 1H2c-.55 0-1 .45-1 1v1c0 .55.45 1 1 1v9c0 .55.45 1 1 1h7c.55 0 1-.45 1-1V5c.55 0 1-.45 1-1V3c0-.55-.45-1-1-1zm-1 12H3V5h1v8h1V5h1v8h1V5h1v8h1V5h1v9zm1-10H2V3h9v1z"/></svg>
          </button>
</form>  </div>

  <div class="file-info">
      291 lines (230 sloc)
      <span class="file-info-divider"></span>
    12.6 KB
  </div>
</div>

    
  <div id="readme" class="readme blob instapaper_body">
    <article class="markdown-body entry-content" itemprop="text"><h1><a id="user-content-pactnet" class="anchor" href="#pactnet" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>PactNet</h1>
<p><a href="https://ci.appveyor.com/project/SEEKJobs/pact-net/branch/master"><img src="https://camo.githubusercontent.com/8dfc8b3da642d3aaaa13fc836efd146b785f0817/68747470733a2f2f63692e6170707665796f722e636f6d2f6170692f70726f6a656374732f7374617475732f35683474396f65726c6871636e776d382f6272616e63682f6d61737465723f7376673d74727565" alt="Build status" data-canonical-src="https://ci.appveyor.com/api/projects/status/5h4t9oerlhqcnwm8/branch/master?svg=true" style="max-width:100%;"></a><br>
A .NET implementation of the Ruby consumer driven contract library, <a href="https://github.com/realestate-com-au/pact">Pact</a>.<br>
Pact is based off the specification found at <a href="https://github.com/bethesque/pact_specification">https://github.com/bethesque/pact_specification</a>.</p>
<p>PactNet primarily provides a fluent .NET DSL for describing HTTP requests that will be made to a service provider and the HTTP responses the consumer expects back to function correctly.<br>
In documenting the consumer interactions, we can replay them on the provider and ensure the provider responds as expected. This basically gives us complete test symmetry and removes the basic need for integrated tests.<br>
PactNet also has the ability to support other mock providers should we see fit.</p>
<p>PactNet is aiming to be Pact Specification Version 1.1 compliant (Version 2 is a WIP).</p>
<p>From the [Pact Specification repo] (<a href="https://github.com/bethesque/pact_specification">https://github.com/bethesque/pact_specification</a>)</p>
<blockquote>
<p>"Pact" is an implementation of "consumer driven contract" testing that allows mocking of responses in the consumer codebase, and verification of the interactions in the provider codebase. The initial implementation was written in Ruby for Rack apps, however a consumer and provider may be implemented in different programming languages, so the "mocking" and the "verifying" steps would be best supported by libraries in their respective project's native languages. Given that the pact file is written in JSON, it should be straightforward to implement a pact library in any language, however, to get the best experience and most reliability of out mixing pact libraries, the matching logic for the requests and responses needs to be identical. There is little confidence to be gained in having your pacts "pass" if the logic used to verify a "pass" is inconsistent between implementations.</p>
</blockquote>
<p>Read more about Pact and the problems it solves at <a href="https://github.com/realestate-com-au/pact">https://github.com/realestate-com-au/pact</a></p>
<p>Please feel free to contribute, we do accept pull requests.</p>
<h2><a id="user-content-usage" class="anchor" href="#usage" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>Usage</h2>
<p>Below are some samples of usage.<br>
We have also written some <code>//NOTE:</code> comments inline in the code to help explain what certain calls do.</p>
<h3><a id="user-content-installing" class="anchor" href="#installing" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>Installing</h3>
<p>Via Nuget with <a href="http://www.nuget.org/packages/PactNet">Install-Package PactNet</a></p>
<h3><a id="user-content-service-consumer" class="anchor" href="#service-consumer" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>Service Consumer</h3>
<h4><a id="user-content-1-build-your-client" class="anchor" href="#1-build-your-client" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>1. Build your client</h4>
<p>Which may look something like this.</p>
<div class="highlight highlight-source-cs"><pre><span class="pl-k">public</span> <span class="pl-k">class</span> <span class="pl-en">SomethingApiClient</span>
{
	<span class="pl-k">public </span><span class="pl-k">string</span> <span class="pl-en">BaseUri</span> { <span class="pl-k">get</span>; <span class="pl-k">set</span>; }

	<span class="pl-k">public</span> <span class="pl-en">SomethingApiClient</span>(string baseUri = null)
	{
		BaseUri = baseUri ?? <span class="pl-s"><span class="pl-pds">"</span>http://my-api<span class="pl-pds">"</span></span>;
	}
	
	<span class="pl-k">public</span> <span class="pl-en">Something</span> <span class="pl-en">GetSomething</span>(<span class="pl-k">string</span> <span class="pl-smi">id</span>)
	{
		<span class="pl-k">string</span> <span class="pl-en">reasonPhrase</span>;
		
		<span class="pl-k">using</span> (<span class="pl-k">var</span> <span class="pl-en">client </span>= <span class="pl-k">new</span> <span class="pl-en">HttpClient</span> { BaseAddress = <span class="pl-k">new</span> <span class="pl-en">Uri</span>(<span class="pl-en">BaseUri</span>) })
		{
			<span class="pl-k">var</span> <span class="pl-en">request </span>= <span class="pl-k">new</span> <span class="pl-en">HttpRequestMessage</span>(HttpMethod.Get, <span class="pl-s"><span class="pl-pds">"</span>/somethings/<span class="pl-pds">"</span></span> + id);
			request.Headers.Add(<span class="pl-s"><span class="pl-pds">"</span>Accept<span class="pl-pds">"</span></span>, <span class="pl-s"><span class="pl-pds">"</span>application/json<span class="pl-pds">"</span></span>);

			<span class="pl-k">var</span> <span class="pl-en">response </span>= client.SendAsync(request);

			<span class="pl-k">var</span> <span class="pl-en">content </span>= response.Result.Content.ReadAsStringAsync().Result;
			<span class="pl-k">var</span> <span class="pl-en">status </span>= response.Result.StatusCode;

			reasonPhrase = response.Result.ReasonPhrase; <span class="pl-c">//NOTE: any Pact mock provider errors will be returned here and in the response body</span>
			
			request.Dispose();
			response.Dispose();
			
			<span class="pl-k">if</span> (status == HttpStatusCode.OK)
			{
				<span class="pl-k">return</span> !String.IsNullOrEmpty(content) ? 
					JsonConvert.DeserializeObject&lt;Something&gt;(<span class="pl-en">content</span>)
					: <span class="pl-c1">null</span>;
			}
		}

		<span class="pl-k">throw</span> <span class="pl-k">new</span> <span class="pl-en">Exception</span>(<span class="pl-en">reasonPhrase</span>); 
	}
}</pre></div>
<h4><a id="user-content-2-describe-and-configure-the-pact-as-a-service-consumer-with-a-mock-service" class="anchor" href="#2-describe-and-configure-the-pact-as-a-service-consumer-with-a-mock-service" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>2. Describe and configure the pact as a service consumer with a mock service</h4>
<p>Create a new test case within your service consumer test project, using whatever test framework you like (in this case we used xUnit).<br>
This should only be instantiated once for the consumer you are testing.</p>
<div class="highlight highlight-source-cs"><pre><span class="pl-k">public</span> <span class="pl-k">class</span> <span class="pl-en">ConsumerMyApiPact</span> : <span class="pl-en">IDisposable</span>
{
	<span class="pl-k">public </span><span class="pl-en">IPactBuilder</span> <span class="pl-en">PactBuilder</span> { <span class="pl-k">get</span>; <span class="pl-en">private</span> <span class="pl-en">set</span>; }
	<span class="pl-k">public </span><span class="pl-en">IMockProviderService</span> <span class="pl-en">MockProviderService</span> { <span class="pl-k">get</span>; <span class="pl-en">private</span> <span class="pl-en">set</span>; }

	<span class="pl-k">public </span><span class="pl-k">int</span> <span class="pl-en">MockServerPort</span> { <span class="pl-k">get</span> { <span class="pl-k">return</span> <span class="pl-c1">1234</span>; } }
	<span class="pl-k">public </span><span class="pl-k">string</span> <span class="pl-en">MockProviderServiceBaseUri</span> { <span class="pl-k">get</span> { <span class="pl-k">return</span> String.Format(<span class="pl-s"><span class="pl-pds">"</span>http://localhost:{0}<span class="pl-pds">"</span></span>, MockServerPort); } }

	<span class="pl-k">public</span> <span class="pl-en">ConsumerMyApiPact</span>()
	{
		PactBuilder = <span class="pl-k">new</span> <span class="pl-en">PactBuilder</span>(); <span class="pl-c">//Uses default directories. PactDir: ..\..\pacts and LogDir: ..\..\logs</span>
		<span class="pl-c">//or</span>
		PactBuilder = <span class="pl-k">new</span> <span class="pl-en">PactBuilder</span>(<span class="pl-k">new</span> <span class="pl-en">PactConfig</span> { PactDir = <span class="pl-s"><span class="pl-pds">@"..\pacts"</span></span>, LogDir = <span class="pl-s"><span class="pl-pds">@"c:\temp\logs"</span></span> }); <span class="pl-c">//Configures the PactDir and/or LogDir.</span>
	
		PactBuilder
			.ServiceConsumer(<span class="pl-s"><span class="pl-pds">"</span>Consumer<span class="pl-pds">"</span></span>)
			.HasPactWith(<span class="pl-s"><span class="pl-pds">"</span>Something API<span class="pl-pds">"</span></span>);

		MockProviderService = PactBuilder.MockService(MockServerPort); <span class="pl-c">//Configure the http mock server</span>
		<span class="pl-c">//or</span>
		MockProviderService = PactBuilder.MockService(MockServerPort, <span class="pl-c1">true</span>); <span class="pl-c">//By passing true as the second param, you can enabled SSL. This will however require a valid SSL certificate installed and bound with netsh (netsh http add sslcert ipport=0.0.0.0:port certhash=thumbprint appid={app-guid}) on the machine running the test. See https://groups.google.com/forum/#!topic/nancy-web-framework/t75dKyfgzpg</span>
		<span class="pl-c">//or</span>
		MockProviderService = PactBuilder.MockService(MockServerPort, bindOnAllAdapters: <span class="pl-c1">true</span>); <span class="pl-c">//By passing true as the bindOnAllAdapters parameter the http mock server will be able to accept external network requests, but will require admin privileges in order to run</span>
		<span class="pl-c">//or</span>
		MockProviderService = PactBuilder.MockService(MockServerPort, <span class="pl-k">new</span> <span class="pl-en">JsonSerializerSettings</span>()); <span class="pl-c">//You can also change the default Json serialization settings using this overload		</span>
	}

	<span class="pl-k">public</span> <span class="pl-k">void</span> <span class="pl-en">Dispose</span>()
	{
		PactBuilder.Build(); <span class="pl-c">//NOTE: Will save the pact file once finished</span>
	}
}

<span class="pl-k">public</span> <span class="pl-k">class</span> <span class="pl-en">SomethingApiConsumerTests</span> : <span class="pl-en">IUseFixture</span>&lt;<span class="pl-en">ConsumerMyApiPact</span>&gt;
{
	<span class="pl-k">private </span><span class="pl-en">IMockProviderService</span> <span class="pl-en">_mockProviderService</span>;
	<span class="pl-k">private </span><span class="pl-k">string</span> <span class="pl-en">_mockProviderServiceBaseUri</span>;
		
	<span class="pl-k">public</span> <span class="pl-k">void</span> <span class="pl-en">SetFixture</span>(<span class="pl-en">ConsumerMyApiPact</span> <span class="pl-smi">data</span>)
	{
		_mockProviderService = data.MockProviderService;
        _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
		data.MockProviderService.ClearInteractions(); <span class="pl-c">//NOTE: Clears any previously registered interactions before the test is run</span>
	}
}</pre></div>
<h4><a id="user-content-3-write-your-test" class="anchor" href="#3-write-your-test" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>3. Write your test</h4>
<p>Create a new test case and implement it.</p>
<div class="highlight highlight-source-cs"><pre><span class="pl-k">public</span> <span class="pl-k">class</span> <span class="pl-en">SomethingApiConsumerTests</span> : <span class="pl-en">IUseFixture</span>&lt;<span class="pl-en">ConsumerMyApiPact</span>&gt;
{
	<span class="pl-k">private </span><span class="pl-en">IMockProviderService</span> <span class="pl-en">_mockProviderService</span>;
	<span class="pl-k">private </span><span class="pl-k">string</span> <span class="pl-en">_mockProviderServiceBaseUri</span>;
		
	<span class="pl-k">public</span> <span class="pl-k">void</span> <span class="pl-en">SetFixture</span>(<span class="pl-en">ConsumerMyApiPact</span> <span class="pl-smi">data</span>)
	{
		_mockProviderService = data.MockProviderService;
        _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
		data.MockProviderService.ClearInteractions(); <span class="pl-c">//NOTE: Clears any previously registered interactions before the test is run</span>
	}
	
	[Fact]
	<span class="pl-k">public</span> <span class="pl-k">void</span> <span class="pl-en">GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething</span>()
	{
		<span class="pl-c">//Arrange</span>
		_mockProviderService
			.Given(<span class="pl-s"><span class="pl-pds">"</span>There is a something with id 'tester'<span class="pl-pds">"</span></span>)
			.UponReceiving(<span class="pl-s"><span class="pl-pds">"</span>A GET request to retrieve the something<span class="pl-pds">"</span></span>)
			.With(<span class="pl-k">new</span> <span class="pl-en">ProviderServiceRequest</span>
			{
				Method = HttpVerb.Get,
				Path = <span class="pl-s"><span class="pl-pds">"</span>/somethings/tester<span class="pl-pds">"</span></span>,
				Headers = <span class="pl-k">new</span> <span class="pl-en">Dictionary</span>&lt;<span class="pl-k">string</span>, <span class="pl-k">string</span>&gt;
				{
					{ <span class="pl-s"><span class="pl-pds">"</span>Accept<span class="pl-pds">"</span></span>, <span class="pl-s"><span class="pl-pds">"</span>application/json<span class="pl-pds">"</span></span> }
				}
			})
			.WillRespondWith(<span class="pl-k">new</span> <span class="pl-en">ProviderServiceResponse</span>
			{
				Status = <span class="pl-c1">200</span>,
				Headers = <span class="pl-k">new</span> <span class="pl-en">Dictionary</span>&lt;<span class="pl-k">string</span>, <span class="pl-k">string</span>&gt;
				{
					{ <span class="pl-s"><span class="pl-pds">"</span>Content-Type<span class="pl-pds">"</span></span>, <span class="pl-s"><span class="pl-pds">"</span>application/json; charset=utf-8<span class="pl-pds">"</span></span> }
				},
				Body = new <span class="pl-c">//NOTE: Note the case sensitivity here, the body will be serialised as per the casing defined</span>
				{
					id = <span class="pl-s"><span class="pl-pds">"</span>tester<span class="pl-pds">"</span></span>,
					firstName = <span class="pl-s"><span class="pl-pds">"</span>Totally<span class="pl-pds">"</span></span>,
					lastName = <span class="pl-s"><span class="pl-pds">"</span>Awesome<span class="pl-pds">"</span></span>
				}
			}); <span class="pl-c">//NOTE: WillRespondWith call must come last as it will register the interaction</span>
			
		<span class="pl-k">var</span> <span class="pl-en">consumer </span>= <span class="pl-k">new</span> <span class="pl-en">SomethingApiClient</span>(_mockProviderServiceBaseUri);
		
		<span class="pl-c">//Act</span>
		<span class="pl-k">var</span> <span class="pl-en">result </span>= consumer.GetSomething(<span class="pl-s"><span class="pl-pds">"</span>tester<span class="pl-pds">"</span></span>);
		
		<span class="pl-c">//Assert</span>
		Assert.Equal(<span class="pl-s"><span class="pl-pds">"</span>tester<span class="pl-pds">"</span></span>, result.id);
		
		_mockProviderService.VerifyInteractions(); <span class="pl-c">//NOTE: Verifies that interactions registered on the mock provider are called once and only once</span>
	}
}</pre></div>
<h4><a id="user-content-4-run-the-test" class="anchor" href="#4-run-the-test" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>4. Run the test</h4>
<p>Everything should be green</p>
<p>Note: we advise using a TDD approach when using this library, however we will leave it up to you.<br>
Likely you will be creating a skeleton client, describing the pact, write the failing test, implement the skeleton client, run the test to make sure it passes, then rinse and repeat.</p>
<h3><a id="user-content-service-provider" class="anchor" href="#service-provider" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>Service Provider</h3>
<h4><a id="user-content-1-create-the-api" class="anchor" href="#1-create-the-api" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>1. Create the API</h4>
<p>You can create the API using whatever framework you like, however we recommend using ASP.NET Web API 2 with Owin as it allows you to use the Microsoft.Owin.Testing Nuget package.<br>
Using the Microsoft.Owin.Testing package allows you to create an in memory version of your API to test against, which removes the pain of deployment and configuration when running the tests.</p>
<h4><a id="user-content-2-tell-the-provider-it-needs-to-honour-the-pact" class="anchor" href="#2-tell-the-provider-it-needs-to-honour-the-pact" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>2. Tell the provider it needs to honour the pact</h4>
<p>Create a new test case within your service provider test project, using whatever test framework you like (in this case we used xUnit).</p>
<p><strong>a)</strong> If using an Owin compatible API, like ASP.NET Web API 2 and using the Microsoft.Owin.Testing Nuget package <strong>(preferred)</strong></p>
<div class="highlight highlight-source-cs"><pre><span class="pl-k">public</span> <span class="pl-k">class</span> <span class="pl-en">SomethingApiTests</span>
{
	[Fact]
	<span class="pl-k">public</span> <span class="pl-k">void</span> <span class="pl-en">EnsureSomethingApiHonoursPactWithConsumer</span>()
	{
		<span class="pl-c">//Arrange</span>
		<span class="pl-en">IPactVerifier</span> <span class="pl-en">pactVerifier</span> = <span class="pl-k">new</span> <span class="pl-en">PactVerifier</span>(() =&gt; {}, () =&gt; {}); <span class="pl-c">//NOTE: You can supply setUp and tearDown, which will run before starting and after completing each individual verification.</span>
		
		pactVerifier
			.ProviderState(<span class="pl-s"><span class="pl-pds">"</span>There is a something with id 'tester'<span class="pl-pds">"</span></span>,
				setUp: AddTesterIfItDoesntExist); <span class="pl-c">//NOTE: We also have tearDown</span>

		<span class="pl-c">//Act / Assert</span>
		<span class="pl-k">using</span> (<span class="pl-k">var</span> <span class="pl-en">testServer </span>= TestServer.Create&lt;Startup&gt;()) <span class="pl-c">//NOTE: This is using the Microsoft.Owin.Testing nuget package</span>
		{
			pactVerifier
				.ServiceProvider(<span class="pl-s"><span class="pl-pds">"</span>Something API<span class="pl-pds">"</span></span>, testServer.HttpClient)
				.HonoursPactWith(<span class="pl-s"><span class="pl-pds">"</span>Consumer<span class="pl-pds">"</span></span>)
				.PactUri(<span class="pl-s"><span class="pl-pds">"</span>../../../Consumer.Tests/pacts/consumer-something_api.json<span class="pl-pds">"</span></span>)
				<span class="pl-c">//or</span>
				.PactUri(<span class="pl-s"><span class="pl-pds">"</span>http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest<span class="pl-pds">"</span></span>) <span class="pl-c">//You can specify a http or https uri</span>
				<span class="pl-c">//or</span>
				.PactUri(<span class="pl-s"><span class="pl-pds">"</span>http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest<span class="pl-pds">"</span></span>, <span class="pl-k">new</span> <span class="pl-en">PactUriOptions</span>(<span class="pl-s"><span class="pl-pds">"</span>someuser<span class="pl-pds">"</span></span>, <span class="pl-s"><span class="pl-pds">"</span>somepassword<span class="pl-pds">"</span></span>)) <span class="pl-c">//You can also specify http/https basic auth details</span>
				.Verify(); <span class="pl-c">//NOTE: Optionally you can control what interactions are verified by specifying a providerDescription and/or providerState</span>
		}
	}

	<span class="pl-k">private</span> <span class="pl-k">void</span> <span class="pl-en">AddTesterIfItDoesntExist</span>()
	{
		<span class="pl-c">//Logic to add the 'tester' something</span>
	}
}</pre></div>
<p>or</p>
<p><strong>b)</strong> If using a non Owin compatible API, you can provide your own HttpClient and point to the deployed API.</p>
<div class="highlight highlight-source-cs"><pre><span class="pl-k">public</span> <span class="pl-k">class</span> <span class="pl-en">SomethingApiTests</span>
{
	[Fact]
	<span class="pl-k">public</span> <span class="pl-k">void</span> <span class="pl-en">EnsureSomethingApiHonoursPactWithConsumer</span>()
	{
		<span class="pl-c">//Arrange</span>
		<span class="pl-en">IPactVerifier</span> <span class="pl-en">pactVerifier</span> = <span class="pl-k">new</span> <span class="pl-en">PactVerifier</span>(() =&gt; {}, () =&gt; {}); <span class="pl-c">//NOTE: The supplied setUp and tearDown actions will run before starting and after completing each individual verification.</span>
		
		pactVerifier
			.ProviderState(<span class="pl-s"><span class="pl-pds">"</span>There is a something with id 'tester'<span class="pl-pds">"</span></span>,
				setUp: AddTesterIfItDoesntExist); <span class="pl-c">//NOTE: We also have tearDown</span>

		<span class="pl-c">//Act / Assert</span>
		<span class="pl-k">using</span> (<span class="pl-k">var</span> <span class="pl-en">client </span>= <span class="pl-k">new</span> <span class="pl-en">HttpClient</span> { BaseAddress = <span class="pl-k">new</span> <span class="pl-en">Uri</span>(<span class="pl-s"><span class="pl-pds">"</span>http://api-address:9999<span class="pl-pds">"</span></span>) })
		{
			pactVerifier
				.ServiceProvider(<span class="pl-s"><span class="pl-pds">"</span>Something API<span class="pl-pds">"</span></span>, client) <span class="pl-c">//NOTE: You can also use your own client by using the ServiceProvider method which takes a Func&lt;ProviderServiceRequest, ProviderServiceResponse&gt;. You are then responsible for mapping and performing the actual provider verification HTTP request within that Func.</span>
				.HonoursPactWith(<span class="pl-s"><span class="pl-pds">"</span>Consumer<span class="pl-pds">"</span></span>)
				.PactUri(<span class="pl-s"><span class="pl-pds">"</span>../../../Consumer.Tests/pacts/consumer-something_api.json<span class="pl-pds">"</span></span>)
				<span class="pl-c">//or</span>
				.PactUri(<span class="pl-s"><span class="pl-pds">"</span>http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest<span class="pl-pds">"</span></span>) <span class="pl-c">//You can specify a http or https uri</span>
				<span class="pl-c">//or</span>
				.PactUri(<span class="pl-s"><span class="pl-pds">"</span>http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest<span class="pl-pds">"</span></span>, <span class="pl-k">new</span> <span class="pl-en">PactUriOptions</span>(<span class="pl-s"><span class="pl-pds">"</span>someuser<span class="pl-pds">"</span></span>, <span class="pl-s"><span class="pl-pds">"</span>somepassword<span class="pl-pds">"</span></span>)) <span class="pl-c">//You can also specify http/https basic auth details</span>
				.Verify(); <span class="pl-c">//NOTE: Optionally you can control what interactions are verified by specifying a providerDescription and/or providerState</span>
		}
	}

	<span class="pl-k">private</span> <span class="pl-k">void</span> <span class="pl-en">AddTesterIfItDoesntExist</span>()
	{
		<span class="pl-c">//Logic to add the 'tester' something</span>
	}
}</pre></div>
<h4><a id="user-content-4-run-the-test-1" class="anchor" href="#4-run-the-test-1" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>4. Run the test</h4>
<p>Everything should be green</p>
<p>Again, please note: we advise using a TDD approach when using this library, however we will leave it up to you.</p>
<p>For further examples please refer to the <a href="https://github.com/SEEK-Jobs/pact-net/tree/master/Samples">Samples</a> in the solution.</p>
<h4><a id="user-content-related-tools" class="anchor" href="#related-tools" aria-hidden="true"><svg aria-hidden="true" class="octicon octicon-link" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M4 9h1v1H4c-1.5 0-3-1.69-3-3.5S2.55 3 4 3h4c1.45 0 3 1.69 3 3.5 0 1.41-.91 2.72-2 3.25V8.59c.58-.45 1-1.27 1-2.09C10 5.22 8.98 4 8 4H4c-.98 0-2 1.22-2 2.5S3 9 4 9zm9-3h-1v1h1c1 0 2 1.22 2 2.5S13.98 12 13 12H9c-.98 0-2-1.22-2-2.5 0-.83.42-1.64 1-2.09V6.25c-1.09.53-2 1.84-2 3.25C6 11.31 7.55 13 9 13h4c1.45 0 3-1.69 3-3.5S14.5 6 13 6z"></path></svg></a>Related Tools</h4>
<p>You might also find the following tool and library helpful:</p>
<ul>
<li><a href="https://github.com/SEEK-Jobs/SEEK.Automation.Phantom">Pact based service simulator</a>: leverage pacts to isolate your services</li>
<li><a href="https://github.com/SEEK-Jobs/seek.automation.stub">Pact based stubbing library</a>: leverage pacts during automation to stub dependencies</li>
</ul>
</article>
  </div>

  </div>

  <button type="button" data-facebox="#jump-to-line" data-facebox-class="linejump" data-hotkey="l" class="d-none">Jump to Line</button>
  <div id="jump-to-line" style="display:none">
    <!-- '"` --><!-- </textarea></xmp> --></option></form><form accept-charset="UTF-8" action="" class="js-jump-to-line-form" method="get"><div style="margin:0;padding:0;display:inline"><input name="utf8" type="hidden" value="&#x2713;" /></div>
      <input class="form-control linejump-input js-jump-to-line-field" type="text" placeholder="Jump to line&hellip;" aria-label="Jump to line" autofocus>
      <button type="submit" class="btn">Go</button>
</form>  </div>

  </div>
  <div class="modal-backdrop js-touch-events"></div>
</div>

    </div>
  </div>

  </div>

      
<div class="container site-footer-container">
  <div class="site-footer " role="contentinfo">
    <ul class="site-footer-links float-right">
        <li><a href="https://github.com/contact" data-ga-click="Footer, go to contact, text:contact">Contact GitHub</a></li>
      <li><a href="https://developer.github.com" data-ga-click="Footer, go to api, text:api">API</a></li>
      <li><a href="https://training.github.com" data-ga-click="Footer, go to training, text:training">Training</a></li>
      <li><a href="https://shop.github.com" data-ga-click="Footer, go to shop, text:shop">Shop</a></li>
        <li><a href="https://github.com/blog" data-ga-click="Footer, go to blog, text:blog">Blog</a></li>
        <li><a href="https://github.com/about" data-ga-click="Footer, go to about, text:about">About</a></li>

    </ul>

    <a href="https://github.com" aria-label="Homepage" class="site-footer-mark" title="GitHub">
      <svg aria-hidden="true" class="octicon octicon-mark-github" height="24" version="1.1" viewBox="0 0 16 16" width="24"><path fill-rule="evenodd" d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.013 8.013 0 0 0 16 8c0-4.42-3.58-8-8-8z"/></svg>
</a>
    <ul class="site-footer-links">
      <li>&copy; 2017 <span title="0.21732s from unicorn-1773539066-6vm0s">GitHub</span>, Inc.</li>
        <li><a href="https://github.com/site/terms" data-ga-click="Footer, go to terms, text:terms">Terms</a></li>
        <li><a href="https://github.com/site/privacy" data-ga-click="Footer, go to privacy, text:privacy">Privacy</a></li>
        <li><a href="https://github.com/security" data-ga-click="Footer, go to security, text:security">Security</a></li>
        <li><a href="https://status.github.com/" data-ga-click="Footer, go to status, text:status">Status</a></li>
        <li><a href="https://help.github.com" data-ga-click="Footer, go to help, text:help">Help</a></li>
    </ul>
  </div>
</div>



  <div id="ajax-error-message" class="ajax-error-message flash flash-error">
    <svg aria-hidden="true" class="octicon octicon-alert" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M8.865 1.52c-.18-.31-.51-.5-.87-.5s-.69.19-.87.5L.275 13.5c-.18.31-.18.69 0 1 .19.31.52.5.87.5h13.7c.36 0 .69-.19.86-.5.17-.31.18-.69.01-1L8.865 1.52zM8.995 13h-2v-2h2v2zm0-3h-2V6h2v4z"/></svg>
    <button type="button" class="flash-close js-flash-close js-ajax-error-dismiss" aria-label="Dismiss error">
      <svg aria-hidden="true" class="octicon octicon-x" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M7.48 8l3.75 3.75-1.48 1.48L6 9.48l-3.75 3.75-1.48-1.48L4.52 8 .77 4.25l1.48-1.48L6 6.52l3.75-3.75 1.48 1.48z"/></svg>
    </button>
    You can't perform that action at this time.
  </div>


    
    <script crossorigin="anonymous" integrity="sha256-nL0D6lZGHoSriVa/d5mpd9bMih+YTHG17LLt/bo9dz8=" src="https://assets-cdn.github.com/assets/frameworks-9cbd03ea56461e84ab8956bf7799a977d6cc8a1f984c71b5ecb2edfdba3d773f.js"></script>
    
    <script async="async" crossorigin="anonymous" integrity="sha256-MpVEs6V4fIUQfgMDkQu7L2eCLO/3/J/ptXlamQDQEwI=" src="https://assets-cdn.github.com/assets/github-329544b3a5787c85107e0303910bbb2f67822ceff7fc9fe9b5795a9900d01302.js"></script>
    
    
    
    
  <div class="js-stale-session-flash stale-session-flash flash flash-warn flash-banner d-none">
    <svg aria-hidden="true" class="octicon octicon-alert" height="16" version="1.1" viewBox="0 0 16 16" width="16"><path fill-rule="evenodd" d="M8.865 1.52c-.18-.31-.51-.5-.87-.5s-.69.19-.87.5L.275 13.5c-.18.31-.18.69 0 1 .19.31.52.5.87.5h13.7c.36 0 .69-.19.86-.5.17-.31.18-.69.01-1L8.865 1.52zM8.995 13h-2v-2h2v2zm0-3h-2V6h2v4z"/></svg>
    <span class="signed-in-tab-flash">You signed in with another tab or window. <a href="">Reload</a> to refresh your session.</span>
    <span class="signed-out-tab-flash">You signed out in another tab or window. <a href="">Reload</a> to refresh your session.</span>
  </div>
  <div class="facebox" id="facebox" style="display:none;">
  <div class="facebox-popup">
    <div class="facebox-content" role="dialog" aria-labelledby="facebox-header" aria-describedby="facebox-description">
    </div>
    <button type="button" class="facebox-close js-facebox-close" aria-label="Close modal">
      <svg aria-hidden="true" class="octicon octicon-x" height="16" version="1.1" viewBox="0 0 12 16" width="12"><path fill-rule="evenodd" d="M7.48 8l3.75 3.75-1.48 1.48L6 9.48l-3.75 3.75-1.48-1.48L4.52 8 .77 4.25l1.48-1.48L6 6.52l3.75-3.75 1.48 1.48z"/></svg>
    </button>
  </div>
</div>


  </body>
</html>

