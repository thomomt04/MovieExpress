Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Enum LocationType
  Server = 1
  Client = 2
End Enum

Public Enum TagRenderMode
  None = 0
  Start = 1
  [End] = 2
  Full = 3
End Enum

Public Enum MessageType
  Information = 1
  [Error] = 2
  Validation = 3
  Success = 4
  Warning = 5
End Enum

Public Enum InputType
  submit
  text
  checkbox
  button
  password
  file
  email
  hidden
  radio
  number
End Enum

Public Enum DefinedButtonType
  General = 0
  Find = 1
  Save = 2
  [New] = 3
  Undo = 4
  Export = 5
  Cancel = 6
End Enum

Public Enum ButtonMainStyle
  NoStyle = 0
  [Default] = 1
  Primary = 2
  Success = 3
  Info = 4
  Warning = 5
  Danger = 6
End Enum

Public Enum ButtonSize
  Normal = 1
  Large = 2
  Small = 3
  ExtraSmall = 4
  Tiny = 5
End Enum

Public Enum BootstrapSize
  <Display(Name:="xs")> ExtraSmall
  <Display(Name:="sm")> Small
  <Display(Name:="md")> Medium
  <Display(Name:="lg")> Large
  <Display(Name:="xl")> ExtraLarge
End Enum

Public Enum ControlSettingType
  All = 1
  Exact = 2
  Label = 3
  Editor = 4
  Display = 5
End Enum

Public Enum KnockoutBindingString
  ''' <summary>
  ''' The property to bind the Value / Selected Value of the control.
  ''' </summary>
  ''' <remarks></remarks>
  value = 1
  checked = 2
  enable = 3
  disable = 4
  click = 5

  ''' <summary>
  ''' Sets the inner text of an html element. Does not work for buttons, use buttonText instead.
  ''' </summary>
  ''' <remarks></remarks>
  text = 6
  options = 7
  optionsValue = 8
  optionsText = 9
  optionsCaption = 10

  optionsGroupList = 12
  groupText = 13
  ''' <summary>
  ''' Creates a Binding Context, where all bindings of controls in this control will be bound under the root of this object.
  ''' </summary>
  [with] = 14

  ''' <summary>
  ''' When must the value be updated? None=On lost focus or use 'afterkeydown'.
  ''' </summary>
  valueUpdate = 16

  ''' <summary>
  ''' Makes an item visible or not. Uses CSS property display: none, not visibility: hidden.
  ''' </summary>
  visible = 17

  ''' <summary>
  ''' Makes an item visible or not with animation.
  ''' </summary>
  visibleA = 18

  ''' <summary>
  ''' Creates a template which will be repeated for each item in the list. Same binding context as 'with' binding.
  ''' </summary>
  ''' <remarks></remarks>
  foreach = 20


  ''' <summary>
  ''' Sets the Button Argument to be sent to the ViewModel when the button is clicked.
  ''' </summary>
  ButtonArgument = 21

  css = 22
  style = 23
  [if] = 24


  ValueLookup = 25
  LookupList = 26
  MinDate = 27
  MaxDate = 28
  Rule = 29
  enableChildren = 30
  DateValue = 31
  Mask = 32
  template = 33
  html = 34
  UID = 35
  DropDown = 36
  dialog = 37
  NValue = 38 'Numeric value
  StarRating = 39
  SValue = 40 'Singular Value, combines Value, Rule and UID
  SChecked = 41
  [event] = 42
  attr = 43
  SCombo = 44
  InlineLabel = 45
  SGrid = 46
  Drag = 47
  Drop = 48
  Captcha = 49
	DateRangeValue = 50
  Slider = 51

  tinyMCE = 52
  tabs = 53
  id_sfx = 54 'ID suffix, use if you want to bind more than 1 control to the same property.
  DateAndTimeEditor = 55
  select2 = 56
  [readonly] = 57


  '******************************************** All the bindings below are 'attribute' bindings ********************************************

  ''' <summary>
  ''' Binds the ID of the control to the property specified. Should only be used in controls (not in custom code).
  ''' </summary>
  id = 100
	''' <summary>
	''' Links a label to an input so when you click the label, the input gets focus. I.E lblUser's lfor='txtUser'.
	''' </summary>
	[for] = 101
	''' <summary>
	''' Tooltip
	''' </summary>
	title = 102
	''' <summary>
	''' Link for anchor tag
	''' </summary>
	href = 103
	''' <summary>
	''' Source for images.
	''' </summary>
	src = 104
	name = 105
	' ''' <summary>
	' ''' Legend / Header / Title of fieldsets.
	' ''' </summary>
	' legend = 106

	itemTitle = 107
	''' <summary>
	''' Placeholder of an Input control
	''' </summary>
  placeholder = 108



End Enum

Public Enum RuleModeType
	Normal = 1
	Cell = 2
End Enum

Public Enum FieldTagType
	span = 1
	strong = 2
	label = 3
End Enum

Public Enum ContainerType
	Div = 1
	Script = 2
End Enum

Public Enum DefinedImageType
	None = -1
	Disk = 0
	BlankPage = 1
	Undo = 2
	Find = 3
	ValidationError = 4
	Left = 5
	Right = 6
	Expand = 7
	Contract = 8
	OpenFile = 9
	Clear = 10
	TrashCan = 11
	Refresh = 12
	Print = 13
	Yes_GreenTick = 14
	No_RedCross = 15
	Help = 16
	Edit = 17
	Redo = 18
	Add = 19
End Enum

Public Enum FontAwesomeIcon
  ''' <summary>
  ''' No image, if you want a blank image in order to bind later, use 'blank'.
  ''' </summary>
  None = -1
  asterisk
  cloud
  envelope
  <Description("envelope-o")> envelopeO
  <Description("address-card-o")> addressCardO
  pencil
  search
  heart
  star
  user
  users
  <Description("user-times")> userTimes
  image
  refresh
  recycle
  <Description("th-large")> grid4
  <Description("th")> grid9
  <Description("th-list")> list
  <Description("check")> tick
  <Description("check-circle")> tickFilled
  warning
  remove
  <Description("search-minus")> zoomout
  <Description("search-plus")> zoomin
  <Description("power-off")> power
  signal
  cog
  cogs
  sliders
  trash
  home
  file
  <Description("file-o")> fileOutline
  <Description("file-pdf-o")> filePdfO
  <Description("floppy-o")> save
  <Description("chevron-left")> back
  <Description("chevron-right")> forward
  <Description("chevron-up")> up
  <Description("chevron-down")> down
  <Description("caret-left")> caretBack
  <Description("caret-right")> caretForward
  <Description("caret-up")> caretUp
  <Description("caret-down")> caretDown
  plus
  <Description("plus-circle")> plusFilled
  minus
  <Description("minus-circle")> minusFilled
  info
  <Description("info-circle")> infoFilled
  question
  <Description("question-circle")> questionFilled
  exclamation
  <Description("exclamation-circle")> exclamationFilled
  <Description("exclamation-triangle")> exclamationTriangle
  <Description("thumbs-up")> thumbsUp
  <Description("thumbs-o-up")> thumbsUpO
  <Description("thumbs-down")> thumbsDown
  <Description("picture-o")> pictureO
  undo
  times
  <Description("times-circle")> timesCircle
  <Description("mail-forward")> redo
  <Description("area-chart")> areaChart
  <Description("pie-chart")> pieChart
  <Description("bar-chart")> barChart
  <Description("line-chart")> lineChart
  link
  eraser
  edit
  blank
  close
  <Description("times-circle-o")> timesCircleO
  <Description("gavel")> auction
  fire
  <Description("credit-card")> creditCard
  <Description("shopping-cart")> shoppingCart
  download
  upload
  <Description("cloud-upload")> cloudUpload
  lock
  unlock
  play
  <Description("forward")> fastforward
  <Description("step-backward")> stepBackward
  print
  copy
  <Description("graduation-cap")> graduationCap
  comment
  comments
  <Description("level-down")> levelDown
  <Description("level-up")> levelUp
  <Description("television")> television
  <Description("video-camera")> videoCamera
  <Description("pencil-square-o")> pencilSquare
  <Description("user-plus")> userPlus
  <Description("minus-square-o")> minusSquareHollow
  <Description("check-square-o")> checkSquareHollow
  <Description("check-circle")> checkCircle
  <Description("check-circle-o")> checkCircleO

  <Description("calendar")> calendar
  <Description("calendar-o")> calendarO
  <Description("calendar-minus-o")> calendarMinusO
  <Description("calendar-plus-o")> calendarPlusO
  <Description("calendar-check-o")> calendarCheckO
  <Description("calendar-times-o")> calendarTimesO
  <Description("plus-square-o")> plusSquareO
  <Description("ticket")> ticket
  sitemap
  eye
  book
  <Description("smile-o")> smileO
  <Description("chain-broken")> chainBroken
  <Description("hand-o-right")> handRight
  flag
  money
  bank
  certificate
  archive
  briefcase
  filter
  umbrella
  circle
  <Description("circle-o")> circleOutline
  headphones
  <Description("code-fork")> codeFork
  bomb
  [stop]
  <Description("mobile")> cellphone
  mobile
  <Description("sign-in")> signIn
  exchange
  <Description("user-md")> doctor
  phone
  <Description("balance-scale")> balanceScale
  eyedropper
  road
  <Description("clock-o")> clockO
  clipboard
  trophy
  'Warning: these require FA v 4.7, make sure you deploy the latest file in \Singular\Files
  <Description("window-restore")> windowRestore
  <Description("window-maximize")> windowMaximise
  <Description("window-minimize")> windowMinimise
  building
  <Description("building-o")> buildingo
  <Description("hospital-o")> hospitalo
  <Description("paint-brush")> paintBrush
	<Description("id-card-o")> idCardO
	<Description("id-card")> idCard
  <Description("shopping-basket")> shoppingBasket

  ' GENERATED BY BWEBBER - UP TO FA 4.7.0
  <Description("align-left")> align_left
  <Description("align-right")> align_right
  amazon
  ambulance
  <Description("american-sign-language-interpreting")> american_sign_language_interpreting
  anchor
  android
  angellist
  <Description("angle-double-down")> angle_double_down
  <Description("angle-double-left")> angle_double_left
  <Description("angle-double-right")> angle_double_right
  <Description("angle-double-up")> angle_double_up
  <Description("angle-down")> angle_down
  <Description("angle-left")> angle_left
  <Description("angle-right")> angle_right
  <Description("angle-up")> angle_up
  apple
  <Description("area-chart")> area_chart
  <Description("arrow-circle-down")> arrow_circle_down
  <Description("arrow-circle-left")> arrow_circle_left
  <Description("arrow-circle-o-down")> arrow_circle_o_down
  <Description("arrow-circle-o-left")> arrow_circle_o_left
  <Description("arrow-circle-o-right")> arrow_circle_o_right
  <Description("arrow-circle-o-up")> arrow_circle_o_up
  <Description("arrow-circle-right")> arrow_circle_right
  <Description("arrow-circle-up")> arrow_circle_up
  <Description("arrow-down")> arrow_down
  <Description("arrow-left")> arrow_left
  <Description("arrow-right")> arrow_right
  <Description("arrow-up")> arrow_up
  arrows
  <Description("arrows-alt")> arrows_alt
  <Description("arrows-h")> arrows_h
  <Description("arrows-v")> arrows_v
  <Description("asl-interpreting")> asl_interpreting
  <Description("assistive-listening-systems")> assistive_listening_systems
  at
  <Description("audio-description")> audio_description
  automobile
  backward
  <Description("balance-scale")> balance_scale
  ban
  bandcamp
  <Description("bar-chart")> bar_chart
  <Description("bar-chart-o")> bar_chart_o
  barcode
  bars
  bath
  bathtub
  battery
  <Description("battery-0")> battery_0
  <Description("battery-1")> battery_1
  <Description("battery-2")> battery_2
  <Description("battery-3")> battery_3
  <Description("battery-4")> battery_4
  <Description("battery-empty")> battery_empty
  <Description("battery-full")> battery_full
  <Description("battery-half")> battery_half
  <Description("battery-quarter")> battery_quarter
  <Description("battery-three-quarters")> battery_three_quarters
  bed
  beer
  behance
  <Description("behance-square")> behance_square
  bell
  <Description("bell-o")> bell_o
  <Description("bell-slash")> bell_slash
  <Description("bell-slash-o")> bell_slash_o
  bicycle
  binoculars
  <Description("birthday-cake")> birthday_cake
  bitbucket
  <Description("bitbucket-square")> bitbucket_square
  bitcoin
  <Description("black-tie")> black_tie
  blind
  bluetooth
  <Description("bluetooth-b")> bluetooth_b
  bold
  bolt
  bookmark
  <Description("bookmark-o")> bookmark_o
  braille
  btc
  bug
  <Description("building-o")> building_o
  bullhorn
  bullseye
  bus
  buysellads
  cab
  calculator
  <Description("calendar-check-o")> calendar_check_o
  <Description("calendar-minus-o")> calendar_minus_o
  <Description("calendar-o")> calendar_o
  <Description("calendar-plus-o")> calendar_plus_o
  <Description("calendar-times-o")> calendar_times_o
  camera
  <Description("camera-retro")> camera_retro
  car
  <Description("caret-down")> caret_down
  <Description("caret-left")> caret_left
  <Description("caret-right")> caret_right
  <Description("caret-square-o-down")> caret_square_o_down
  <Description("caret-square-o-left")> caret_square_o_left
  <Description("caret-square-o-right")> caret_square_o_right
  <Description("caret-square-o-up")> caret_square_o_up
  <Description("caret-up")> caret_up
  <Description("cart-arrow-down")> cart_arrow_down
  <Description("cart-plus")> cart_plus
  cc
  <Description("cc-amex")> cc_amex
  <Description("cc-diners-club")> cc_diners_club
  <Description("cc-discover")> cc_discover
  <Description("cc-jcb")> cc_jcb
  <Description("cc-mastercard")> cc_mastercard
  <Description("cc-paypal")> cc_paypal
  <Description("cc-stripe")> cc_stripe
  <Description("cc-visa")> cc_visa
  chain
  <Description("chain-broken")> chain_broken
  check
  <Description("check-circle")> check_circle
  <Description("check-circle-o")> check_circle_o
  <Description("check-square")> check_square
  <Description("check-square-o")> check_square_o
  <Description("chevron-circle-down")> chevron_circle_down
  <Description("chevron-circle-left")> chevron_circle_left
  <Description("chevron-circle-right")> chevron_circle_right
  <Description("chevron-circle-up")> chevron_circle_up
  <Description("chevron-down")> chevron_down
  <Description("chevron-left")> chevron_left
  <Description("chevron-right")> chevron_right
  <Description("chevron-up")> chevron_up
  child
  chrome
  <Description("circle-o")> circle_o
  <Description("circle-o-notch")> circle_o_notch
  <Description("circle-thin")> circle_thin
  <Description("clock-o")> clock_o
  clone
  <Description("cloud-download")> cloud_download
  <Description("cloud-upload")> cloud_upload
  cny
  code
  <Description("code-fork")> code_fork
  codepen
  codiepie
  coffee
  columns
  <Description("comment-o")> comment_o
  commenting
  <Description("commenting-o")> commenting_o
  <Description("comments-o")> comments_o
  compass
  compress
  connectdevelop
  contao
  copyright
  <Description("creative-commons")> creative_commons
  <Description("credit-card")> credit_card
  <Description("credit-card-alt")> credit_card_alt
  crop
  crosshairs
  css3
  cube
  cubes
  cut
  cutlery
  dashboard
  dashcube
  database
  deaf
  deafness
  dedent
  delicious
  desktop
  deviantart
  diamond
  digg
  dollar
  <Description("dot-circle-o")> dot_circle_o
  dribbble
  <Description("drivers-license")> drivers_license
  <Description("drivers-license-o")> drivers_license_o
  dropbox
  drupal
  edge
  eercast
  eject
  <Description("ellipsis-h")> ellipsis_h
  <Description("ellipsis-v")> ellipsis_v
  empire
  <Description("envelope-o")> envelope_o
  <Description("envelope-open")> envelope_open
  <Description("envelope-open-o")> envelope_open_o
  <Description("envelope-square")> envelope_square
  envira
  etsy
  eur
  euro
  <Description("exclamation-circle")> exclamation_circle
  <Description("exclamation-triangle")> exclamation_triangle
  expand
  expeditedssl
  <Description("external-link")> external_link
  <Description("external-link-square")> external_link_square
  <Description("eye-slash")> eye_slash
  fa
  facebook
  <Description("facebook-f")> facebook_f
  <Description("facebook-official")> facebook_official
  <Description("facebook-square")> facebook_square
  <Description("fast-backward")> fast_backward
  <Description("fast-forward")> fast_forward
  fax
  feed
  female
  <Description("fighter-jet")> fighter_jet
  <Description("file-archive-o")> file_archive_o
  <Description("file-audio-o")> file_audio_o
  <Description("file-code-o")> file_code_o
  <Description("file-excel-o")> file_excel_o
  <Description("file-image-o")> file_image_o
  <Description("file-movie-o")> file_movie_o
  <Description("file-o")> file_o
  <Description("file-pdf-o")> file_pdf_o
  <Description("file-photo-o")> file_photo_o
  <Description("file-picture-o")> file_picture_o
  <Description("file-powerpoint-o")> file_powerpoint_o
  <Description("file-sound-o")> file_sound_o
  <Description("file-text")> file_text
  <Description("file-text-o")> file_text_o
  <Description("file-video-o")> file_video_o
  <Description("file-word-o")> file_word_o
  <Description("file-zip-o")> file_zip_o
  <Description("files-o")> files_o
  film
  <Description("fire-extinguisher")> fire_extinguisher
  firefox
  <Description("first-order")> first_order
  <Description("flag-checkered")> flag_checkered
  <Description("flag-o")> flag_o
  flash
  flask
  flickr
  <Description("floppy-o")> floppy_o
  folder
  <Description("folder-o")> folder_o
  <Description("folder-open")> folder_open
  <Description("folder-open-o")> folder_open_o
  font
  <Description("font-awesome")> font_awesome
  fonticons
  <Description("fort-awesome")> fort_awesome
  forumbee
  foursquare
  <Description("free-code-camp")> free_code_camp
  <Description("frown-o")> frown_o
  <Description("futbol-o")> futbol_o
  gamepad
  gavel
  gbp
  ge
  gear
  gears
  genderless
  <Description("get-pocket")> get_pocket
  gg
  <Description("gg-circle")> gg_circle
  gift
  git
  <Description("git-square")> git_square
  github
  <Description("github-alt")> github_alt
  <Description("github-square")> github_square
  gitlab
  gittip
  glass
  glide
  <Description("glide-g")> glide_g
  globe
  google
  <Description("google-plus")> google_plus
  <Description("google-plus-circle")> google_plus_circle
  <Description("google-plus-official")> google_plus_official
  <Description("google-plus-square")> google_plus_square
  <Description("google-wallet")> google_wallet
  <Description("graduation-cap")> graduation_cap
  gratipay
  grav
  group
  <Description("h-square")> h_square
  <Description("hacker-news")> hacker_news
  <Description("hand-grab-o")> hand_grab_o
  <Description("hand-lizard-o")> hand_lizard_o
  <Description("hand-o-down")> hand_o_down
  <Description("hand-o-left")> hand_o_left
  <Description("hand-o-right")> hand_o_right
  <Description("hand-o-up")> hand_o_up
  <Description("hand-paper-o")> hand_paper_o
  <Description("hand-peace-o")> hand_peace_o
  <Description("hand-pointer-o")> hand_pointer_o
  <Description("hand-rock-o")> hand_rock_o
  <Description("hand-scissors-o")> hand_scissors_o
  <Description("hand-spock-o")> hand_spock_o
  <Description("hand-stop-o")> hand_stop_o
  <Description("handshake-o")> handshake_o
  <Description("hard-of-hearing")> hard_of_hearing
  hashtag
  <Description("hdd-o")> hdd_o
  header
  <Description("heart-o")> heart_o
  heartbeat
  history
  <Description("hospital-o")> hospital_o
  hotel
  hourglass
  <Description("hourglass-1")> hourglass_1
  <Description("hourglass-2")> hourglass_2
  <Description("hourglass-3")> hourglass_3
  <Description("hourglass-end")> hourglass_end
  <Description("hourglass-half")> hourglass_half
  <Description("hourglass-o")> hourglass_o
  <Description("hourglass-start")> hourglass_start
  houzz
  html5
  <Description("i-cursor")> i_cursor
  <Description("id-badge")> id_badge
  <Description("id-card")> id_card
  <Description("id-card-o")> id_card_o
  ils
  imdb
  inbox
  indent
  industry
  <Description("info-circle")> info_circle
  inr
  instagram
  institution
  <Description("internet-explorer")> internet_explorer
  intersex
  ioxhost
  italic
  joomla
  jpy
  jsfiddle
  key
  <Description("keyboard-o")> keyboard_o
  krw
  language
  laptop
  lastfm
  <Description("lastfm-square")> lastfm_square
  leaf
  leanpub
  legal
  <Description("lemon-o")> lemon_o
  <Description("level-down")> level_down
  <Description("level-up")> level_up
  <Description("life-bouy")> life_bouy
  <Description("life-buoy")> life_buoy
  <Description("life-ring")> life_ring
  <Description("life-saver")> life_saver
  <Description("lightbulb-o")> lightbulb_o
  <Description("line-chart")> line_chart
  linkedin
  <Description("linkedin-square")> linkedin_square
  linode
  linux
  <Description("list-alt")> list_alt
  <Description("list-ol")> list_ol
  <Description("list-ul")> list_ul
  <Description("location-arrow")> location_arrow
  <Description("long-arrow-down")> long_arrow_down
  <Description("long-arrow-left")> long_arrow_left
  <Description("long-arrow-right")> long_arrow_right
  <Description("long-arrow-up")> long_arrow_up
  <Description("low-vision")> low_vision
  magic
  magnet
  <Description("mail-forward")> mail_forward
  <Description("mail-reply")> mail_reply
  <Description("mail-reply-all")> mail_reply_all
  male
  map
  <Description("map-marker")> map_marker
  <Description("map-o")> map_o
  <Description("map-pin")> map_pin
  <Description("map-signs")> map_signs
  mars
  <Description("mars-double")> mars_double
  <Description("mars-stroke")> mars_stroke
  <Description("mars-stroke-h")> mars_stroke_h
  <Description("mars-stroke-v")> mars_stroke_v
  maxcdn
  meanpath
  medium
  medkit
  meetup
  <Description("meh-o")> meh_o
  mercury
  microchip
  microphone
  <Description("microphone-slash")> microphone_slash
  <Description("minus-circle")> minus_circle
  <Description("minus-square")> minus_square
  <Description("minus-square-o")> minus_square_o
  mixcloud
  <Description("mobile-phone")> mobile_phone
  modx
  <Description("moon-o")> moon_o
  <Description("mortar-board")> mortar_board
  motorcycle
  <Description("mouse-pointer")> mouse_pointer
  music
  navicon
  neuter
  <Description("newspaper-o")> newspaper_o
  <Description("object-group")> object_group
  <Description("object-ungroup")> object_ungroup
  odnoklassniki
  <Description("odnoklassniki-square")> odnoklassniki_square
  opencart
  openid
  opera
  <Description("optin-monster")> optin_monster
  outdent
  pagelines
  <Description("paint-brush")> paint_brush
  <Description("paper-plane")> paper_plane
  <Description("paper-plane-o")> paper_plane_o
  paperclip
  paragraph
  paste
  pause
  <Description("pause-circle")> pause_circle
  <Description("pause-circle-o")> pause_circle_o
  paw
  paypal
  <Description("pencil-square")> pencil_square
  <Description("pencil-square-o")> pencil_square_o
  percent
  <Description("phone-square")> phone_square
  photo
  <Description("picture-o")> picture_o
  <Description("pie-chart")> pie_chart
  <Description("pied-piper")> pied_piper
  <Description("pied-piper-alt")> pied_piper_alt
  <Description("pied-piper-pp")> pied_piper_pp
  pinterest
  <Description("pinterest-p")> pinterest_p
  <Description("pinterest-square")> pinterest_square
  plane
  <Description("play-circle")> play_circle
  <Description("play-circle-o")> play_circle_o
  plug
  <Description("plus-circle")> plus_circle
  <Description("plus-square")> plus_square
  <Description("plus-square-o")> plus_square_o
  podcast
  <Description("power-off")> power_off
  <Description("product-hunt")> product_hunt
  <Description("puzzle-piece")> puzzle_piece
  qq
  qrcode
  <Description("question-circle")> question_circle
  <Description("question-circle-o")> question_circle_o
  quora
  <Description("quote-left")> quote_left
  <Description("quote-right")> quote_right
  ra
  random
  ravelry
  rebel
  reddit
  <Description("reddit-alien")> reddit_alien
  <Description("reddit-square")> reddit_square
  registered
  renren
  reorder
  repeat
  reply
  <Description("reply-all")> reply_all
  resistance
  retweet
  rmb
  rocket
  <Description("rotate-left")> rotate_left
  <Description("rotate-right")> rotate_right
  rouble
  rss
  <Description("rss-square")> rss_square
  rub
  ruble
  rupee
  s15
  safari
  scissors
  scribd
  <Description("search-minus")> search_minus
  <Description("search-plus")> search_plus
  sellsy
  send
  <Description("send-o")> send_o
  server
  share
  <Description("share-alt")> share_alt
  <Description("share-alt-square")> share_alt_square
  <Description("share-square")> share_square
  <Description("share-square-o")> share_square_o
  shekel
  sheqel
  shield
  ship
  shirtsinbulk
  <Description("shopping-bag")> shopping_bag
  <Description("shopping-basket")> shopping_basket
  <Description("shopping-cart")> shopping_cart
  shower
  <Description("sign-in")> sign_in
  <Description("sign-language")> sign_language
  <Description("sign-out")> sign_out
  signing
  simplybuilt
  skyatlas
  skype
  slack
  slideshare
  <Description("smile-o")> smile_o
  snapchat
  <Description("snapchat-ghost")> snapchat_ghost
  <Description("snapchat-square")> snapchat_square
  <Description("snowflake-o")> snowflake_o
  <Description("soccer-ball-o")> soccer_ball_o
  sort
  <Description("sort-alpha-asc")> sort_alpha_asc
  <Description("sort-alpha-desc")> sort_alpha_desc
  <Description("sort-amount-asc")> sort_amount_asc
  <Description("sort-amount-desc")> sort_amount_desc
  <Description("sort-asc")> sort_asc
  <Description("sort-desc")> sort_desc
  <Description("sort-down")> sort_down
  <Description("sort-numeric-asc")> sort_numeric_asc
  <Description("sort-numeric-desc")> sort_numeric_desc
  <Description("sort-up")> sort_up
  soundcloud
  <Description("space-shuttle")> space_shuttle
  spinner
  spoon
  spotify
  square
  <Description("square-o")> square_o
  <Description("stack-exchange")> stack_exchange
  <Description("stack-overflow")> stack_overflow
  <Description("star-half")> star_half
  <Description("star-half-empty")> star_half_empty
  <Description("star-half-full")> star_half_full
  <Description("star-half-o")> star_half_o
  <Description("star-o")> star_o
  steam
  <Description("steam-square")> steam_square
  <Description("step-backward")> step_backward
  <Description("step-forward")> step_forward
  stethoscope
  <Description("sticky-note")> sticky_note
  <Description("sticky-note-o")> sticky_note_o
  <Description("stop-circle")> stop_circle
  <Description("stop-circle-o")> stop_circle_o
  <Description("street-view")> street_view
  strikethrough
  stumbleupon
  <Description("stumbleupon-circle")> stumbleupon_circle
  subscript
  subway
  suitcase
  <Description("sun-o")> sun_o
  superpowers
  superscript
  support
  table
  tablet
  tachometer
  tag
  tags
  tasks
  taxi
  telegram
  <Description("tencent-weibo")> tencent_weibo
  terminal
  <Description("text-height")> text_height
  <Description("text-width")> text_width
  th
  <Description("th-large")> th_large
  <Description("th-list")> th_list
  themeisle
  thermometer
  <Description("thermometer-0")> thermometer_0
  <Description("thermometer-1")> thermometer_1
  <Description("thermometer-2")> thermometer_2
  <Description("thermometer-3")> thermometer_3
  <Description("thermometer-4")> thermometer_4
  <Description("thermometer-empty")> thermometer_empty
  <Description("thermometer-full")> thermometer_full
  <Description("thermometer-half")> thermometer_half
  <Description("thermometer-quarter")> thermometer_quarter
  <Description("thermometer-three-quarters")> thermometer_three_quarters
  <Description("thumb-tack")> thumb_tack
  <Description("thumbs-down")> thumbs_down
  <Description("thumbs-o-down")> thumbs_o_down
  <Description("thumbs-o-up")> thumbs_o_up
  <Description("thumbs-up")> thumbs_up
  <Description("times-circle")> times_circle
  <Description("times-circle-o")> times_circle_o
  <Description("times-rectangle")> times_rectangle
  <Description("times-rectangle-o")> times_rectangle_o
  tint
  <Description("toggle-down")> toggle_down
  <Description("toggle-left")> toggle_left
  <Description("toggle-off")> toggle_off
  <Description("toggle-on")> toggle_on
  <Description("toggle-right")> toggle_right
  <Description("toggle-up")> toggle_up
  trademark
  train
  transgender
  <Description("transgender-alt")> transgender_alt
  <Description("trash-o")> trash_o
  tree
  trello
  tripadvisor
  truck
  [try]
  tty
  tumblr
  <Description("tumblr-square")> tumblr_square
  <Description("turkish-lira")> turkish_lira
  tv
  twitch
  twitter
  <Description("twitter-square")> twitter_square
  underline
  <Description("universal-access")> universal_access
  university
  unlink
  <Description("unlock-alt")> unlock_alt
  unsorted
  usb
  usd
  <Description("user-circle")> user_circle
  <Description("user-circle-o")> user_circle_o
  <Description("user-md")> user_md
  <Description("user-o")> user_o
  <Description("user-plus")> user_plus
  <Description("user-secret")> user_secret
  <Description("user-times")> user_times
  vcard
  <Description("vcard-o")> vcard_o
  venus
  <Description("venus-double")> venus_double
  <Description("venus-mars")> venus_mars
  viacoin
  viadeo
  <Description("viadeo-square")> viadeo_square
  <Description("video-camera")> video_camera
  vimeo
  <Description("vimeo-square")> vimeo_square
  vine
  vk
  <Description("volume-control-phone")> volume_control_phone
  <Description("volume-down")> volume_down
  <Description("volume-off")> volume_off
  <Description("volume-up")> volume_up
  wechat
  weibo
  weixin
  whatsapp
  wheelchair
  <Description("wheelchair-alt")> wheelchair_alt
  wifi
  <Description("wikipedia-w")> wikipedia_w
  <Description("window-close")> window_close
  <Description("window-close-o")> window_close_o
  <Description("window-maximize")> window_maximize
  <Description("window-minimize")> window_minimize
  <Description("window-restore")> window_restore
  windows
  won
  wordpress
  wpbeginner
  wpexplorer
  wpforms
  wrench
  xing
  <Description("xing-square")> xing_square
  <Description("y-combinator")> y_combinator
  <Description("y-combinator-square")> y_combinator_square
  yahoo
  yc
  <Description("yc-square")> yc_square
  yelp
  yen
  yoast
  youtube
  <Description("youtube-play")> youtube_play
  <Description("youtube-square")> youtube_square
End Enum

'NOTE: Make sure Singular.DataAnnotations.HorizontalAlign matches this enum
Public Enum TextAlign
  left = 1
  center = 2
  right = 3
  justify = 4
End Enum

Public Enum VerticalAlign
  top = 1
  bottom = 2
  baseline = 3
  middle = 4
End Enum

Public Enum Display
  block = 1
  inline = 2
  <System.ComponentModel.Description("inline-block")>
  inlineblock = 3
  none = 4
  table = 5
  <System.ComponentModel.Description("table-row")>
  tablerow = 6
  <System.ComponentModel.Description("table-cell")>
  tablecell = 7
End Enum

Public Enum Position
  absolute
  fixed
  inherit
  initial
  relative
  [static]
End Enum

Public Enum Overflow
  auto
  hidden
  scroll
  visible
  initial
  inherit
End Enum

Public Enum FontWeight
  <System.ComponentModel.Description("300")> light
  normal 'same as 400
  <System.ComponentModel.Description("600")> semiBold
  bold 'same as 700
  <System.ComponentModel.Description("800")> heavy
End Enum

Public Enum PostBackType
  None = 1
  Full = 2
  Ajax = 3
End Enum

Public Enum ButtonStyle
  JQuery = 1
  Bootstrap = 2
End Enum

Public Enum VisibleFadeType
  ''' <summary>
  ''' Changes state without any animation
  ''' </summary>
  None = 1
  ''' <summary>
  ''' Fades in / out using opacity
  ''' </summary>
  Fade = 2
  ''' <summary>
  ''' Fades in / out by changing the height 
  ''' </summary>
  SlideUpDown = 3
  ''' <summary>
  ''' Fly's in from the Left, out to the Right.
  ''' </summary>
  FlyInOut = 4
End Enum

Public Enum LinkTargetType
  NotSet
  _blank
  _self
  _parent
  _top
End Enum

Public Enum SaveMode
  OnSaveClick = 1
  Immediate = 2
End Enum

Public Enum ValidationDisplayMode
  None = 0
  Controls = 1
  SubmitMessage = 2
  ValidationSummary = 4
  Normal = 7
  NoSubmitMessage = 5
  ControlsEnhanced = 8
End Enum

Public Enum ValidationMode
  OnFirstChange = 1
  OnLoad = 2
  OnSubmit = 4
End Enum

'Public Enum DeserialiseMode
'  ''' <summary>
'  ''' Delete items not returned from server.
'  ''' </summary>
'  Normal = 1
'  ''' <summary>
'  ''' Leave items, only update / add items returned from server
'  ''' </summary>
'  Stateless = 2
'End Enum

Public Class BootstrapEnums

  Public Enum Style
    DefaultStyle = 1
    Primary = 2
    Success = 3
    Info = 4
    Warning = 5
    Danger = 6
    Custom = 7
  End Enum

  Public Enum NavbarStyle
    [Default] = 1
    Inverted = 2
  End Enum

  Public Enum ButtonSize
    ExtraSmall = 1
    Small = 2
    Medium = 3
    Large = 4
    Custom = 5
  End Enum

  Public Enum WellSize
    [Default] = 1
    Small = 2
    Large = 3
    Custom = 4
  End Enum

  Public Enum FormGroupSize
    Small = 1
    Large = 2
    Custom = 3
    ExtraSmall = 4
  End Enum

  Public Enum InputGroupSize
    Small = 1
    Large = 2
    Custom = 3
    ExtraSmall = 4
  End Enum

  Public Enum PagerPosition
    Bottom = 1
    Top = 2
    None = 3
  End Enum

  Public Enum FDTileColor
    Green = 1
    Lemon = 2
    Red = 3
    Blue = 4
    Orange = 5
    Prusia = 6
    Concrete = 7
    Purple = 8
  End Enum

  Public Enum PortletColor
    White = 1
    Primary = 2
    Info = 3
    Dark = 4
    Success = 5
    Danger = 6
  End Enum

  Public Enum InputSize
    Small = 1
    Large = 2
    Custom = 3
    ExtraSmall = 4
  End Enum

  Public Enum FlatDreamAlertColor
    Primary = 1
    Success = 2
    Info = 3
    Warning = 4
    Danger = 5
    Custom = 6
  End Enum

  Public Enum TabAlignment
    Top = 1
    Left = 2
    Right = 3
  End Enum

End Class