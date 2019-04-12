var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/advertisement/',
  withCredentials: true
});

var data = {
  siteId: utils.getQueryInt('siteId'),
  advertisementType: utils.getQueryString('advertisementType'),
  pageLoad: false,
  pageAlert: null,
  advertisementInfoList: null,
  types: null,
};

var methods = {
  apiGetList: function () {
    var $this = this;

    if ($this.pageLoad) utils.loading(true);
    $api.get('', {
      params: {
        siteId: $this.siteId,
        advertisementType: $this.advertisementType
      }
    }).then(function (response) {
      var res = response.data;

      $this.advertisementInfoList = res.value;
      $this.types = res.types;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
    });
  },

  apiDelete: function (item) {
    var $this = this;

    utils.loading(true);
    $api.delete('', {
      params: {
        siteId: $this.siteId,
        advertisementId: item.id,
        advertisementType: $this.advertisementType
      }
    }).then(function (response) {
      var res = response.data;

      $this.advertisementInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnAddClick: function (item) {
    location.href = utils.getPageUrl('advertisementAdd.html');
  },

  btnEditClick: function (item) {
    location.href = utils.getPageUrl('advertisementAdd.html') + '&advertisementId=' + item.id;
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除漂浮广告',
      text: '此操作将删除漂浮广告' + item.advertisementName + '，确定吗？',
      callback: function () {
        $this.apiDelete(item);
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  watch: {
    advertisementType: function (val, oldVal) {
      this.apiGetList();
    }
  },
  methods: methods,
  created: function () {
    this.apiGetList();
  }
});
