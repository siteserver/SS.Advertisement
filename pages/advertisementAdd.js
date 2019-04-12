var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/advertisementAdd/',
  withCredentials: true
});

var $typeBase = 'Base';
var $typeFloatImage = 'FloatImage';
var $typeScreenDown = 'ScreenDown';
var $typeOpenWindow = 'OpenWindow';
var $typeDone = 'Done';

var data = {
  siteId: utils.getQueryInt('siteId'),
  advertisementId: utils.getQueryInt('advertisementId'),
  pageLoad: false,
  pageAlert: null,
  pageType: $typeBase,
  adInfo: null,
  advertisementTypes: null,
  channels: null,
  fileTemplates: null,
  positionTypes: null,
  rollingTypes: null,
  uploadUrl: null,
  files: []
};

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get('', {
      params: {
        siteId: $this.siteId,
        advertisementId: $this.advertisementId
      }
    }).then(function (response) {
      var res = response.data;

      $this.adInfo = res.value;
      $this.advertisementTypes = res.advertisementTypes;
      $this.channels = res.channels;
      $this.fileTemplates = res.fileTemplates;
      $this.positionTypes = res.positionTypes;
      $this.rollingTypes = res.rollingTypes;
      $this.uploadUrl = $api.defaults.baseURL + 'actions/upload?adminToken=' + res.adminToken + '&siteId=' + $this.siteId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post('?siteId=' + this.siteId, this.adInfo).then(function (response) {
      var res = response.data;

      $this.pageType = $typeDone;

      setTimeout(function() {
        location.href = utils.getPageUrl('advertisement.html');
      }, 1500);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  inputFile: function(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.upload.active) {
        this.$refs.upload.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.adInfo.imageUrl = newFile.response.value;
      this.adInfo.width = newFile.response.width;
      this.adInfo.height = newFile.response.height;
    }
  },

  inputFilter: function (newFile, oldFile, prevent) {
    if (newFile && !oldFile) {
      if (!/\.(gif|jpg|jpeg|png|webp)$/i.test(newFile.name)) {
        swal2({
          title: '上传格式错误！',
          text: '请上传图片',
          type: 'error',
          confirmButtonText: '确 定',
          confirmButtonClass: 'btn btn-primary',
        });
        return prevent()
      }
    }
  },

  btnSelectClick: function () {
    utils.openLayer({
      title: '选择图片',
      url: '../../cms/modalSelectImage.aspx?siteId=' + this.siteId + '&textBoxClientID=imageUrl'
    });
  },

  btnUploadClick: function () {
    utils.openLayer({
      title: '上传图片',
      url: '../../cms/modalUploadImageSingle.aspx?siteId=' + this.siteId + '&textBoxClientID=imageUrl'
    });
  },

  btnPreviousClick: function () {
    this.pageType = $typeBase;
    utils.up();
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validateAll('adInfo').then(function (result) {
      if (result) {
        if ($this.pageType === $typeBase) {
          $this.pageType = $this.adInfo.advertisementType;
          utils.up();
        } else {
          $this.apiSubmit();
        }
      }
    });
  }
};

Vue.component("date-picker", window.DatePicker.default);

new Vue({
  el: '#main',
  data: data,
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
