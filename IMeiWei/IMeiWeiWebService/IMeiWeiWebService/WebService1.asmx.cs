using IMeiWei.Model;
using IMeiWeiWebService.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace IMeiWeiWebService
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string HelloWorld()
        {
            return "{'result':'Hello World'}";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Registration(string phoneNumber, string password, string securityNumber, string userName)
        {
            IMeiWei.BLL.user bll_user = new IMeiWei.BLL.user();
            IMeiWei.Model.user model_user = new IMeiWei.Model.user();
            List<IMeiWei.Model.user> old_model_users = new List<IMeiWei.Model.user>();

            old_model_users = bll_user.GetModelList(" phoneNumber = '" + phoneNumber + "'");

            if (null != old_model_users && old_model_users.Count > 0)
            {
                return "DUPLICATE";
            }

            model_user.IsActive = false;
            model_user.Password = password;
            model_user.SecurityCode = securityNumber;
            model_user.UserName = userName;
            model_user.UserStatusId = 1;
            model_user.PhoneNumber = phoneNumber;

            bll_user.Add(model_user);

            return "SUCCESS";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Login(string phoneNumber, string password)
        {
            IMeiWei.BLL.user bll_user = new IMeiWei.BLL.user();
            IMeiWei.Model.user model_user = new IMeiWei.Model.user();
            string result = string.Empty;

            model_user = bll_user.GetModelList(" phoneNumber = '" + phoneNumber + "'").FirstOrDefault();

            if (null != model_user)
            {
                if (model_user.Password != password)
                {
                    result = "PASSWORD_ERROR";
                }
                else
                {
                    string tokenId = Guid.NewGuid().ToString();
                    //Session.Add(tokenId, phoneNumber);
                    result = tokenId;
                }
            }
            else
            {
                result = "USERNAME_ERROR";
            }

            var js = GenJSSerializer();
            return js.Serialize(result);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateBusiness(
            string businessId, string businessName, string businessAddress, string businessPhone,
            string businessHours, string percapitaConsumption, string restauranThighLights, string firstOffer,
            string vipOffer, string businessTypeid, string areaOneId, string areaTwoId, string caixiOneId,
            string caixiTwoId, string scencOneId, string isPark, string isAllelic, string senderTypeId, string senderTypeContent)
        {
            IMeiWei.BLL.business bll_business = new IMeiWei.BLL.business();
            var model_business = bll_business.GetModelList(" BusinessId = '" + businessId + "'").FirstOrDefault();
            bool isNew = false;
            if (null == model_business)
            {
                model_business = new IMeiWei.Model.business();
                model_business.BusinessId = businessId;
                isNew = true;
            }
            model_business.BusinessName = businessName;
            model_business.BusinessAddress = businessAddress;
            model_business.BusinessPhone = businessPhone;
            model_business.BusinessTypeId = businessTypeid;
            model_business.BusinessHours = businessHours;
            model_business.FirstOffer = firstOffer;
            model_business.PerCapitaConsumption = Convert.ToInt32(percapitaConsumption);
            model_business.VIPOffer = vipOffer;
            model_business.RestaurantHighlights = restauranThighLights;
            if (isAllelic == "undefined" || string.IsNullOrEmpty(isAllelic))
            {
                model_business.IsServerAllelic = false;
            }
            else
            {
                model_business.IsServerAllelic = isAllelic == "0" ? isAllelic != "0" : Convert.ToBoolean(isAllelic);
            }
            if (isPark == "undefined" || string.IsNullOrEmpty(isPark))
            {
                model_business.IsServerPark = false;
            }
            else
            {
                model_business.IsServerPark = isPark == "0" ? isPark != "0" : Convert.ToBoolean(isPark);
            }
            model_business.BusinessSenderContent = senderTypeContent;
            model_business.BusinessSenderTypeId = senderTypeId;

            if (isNew)
            {
                bll_business.Add(model_business);
            }
            else
            {
                bll_business.Update(model_business);
            }

            IMeiWei.BLL.businessselectitem bll_selectItem = new IMeiWei.BLL.businessselectitem();
            IMeiWei.BLL.businessselectdetail bll_selectDetail = new IMeiWei.BLL.businessselectdetail();
            IMeiWei.Model.businessselectitem model_selectItem_caixiOne = new businessselectitem();
            IMeiWei.Model.businessselectitem model_selectItem_caixiTwo = new businessselectitem();
            IMeiWei.Model.businessselectitem model_selectItem_areaOne = new businessselectitem();
            IMeiWei.Model.businessselectitem model_selectItem_areaTwo = new businessselectitem();
            IMeiWei.Model.businessselectitem model_selectItem_sceneOne = new businessselectitem();

            IMeiWei.Model.businessselectdetail model_selectDetail_caixiOne = new businessselectdetail();
            IMeiWei.Model.businessselectdetail model_selectDetail_caixiTwo = new businessselectdetail();
            IMeiWei.Model.businessselectdetail model_selectDetail_areaOne = new businessselectdetail();
            IMeiWei.Model.businessselectdetail model_selectDetail_areaTwo = new businessselectdetail();
            IMeiWei.Model.businessselectdetail model_selectDetail_sceneOne = new businessselectdetail();

            BusinessSelectItemProvider _businessSelectItemProvider = new BusinessSelectItemProvider();
            var selectItems = _businessSelectItemProvider.LoadBusinessSelectItem(businessTypeid);

            model_selectItem_caixiOne = selectItems.FirstOrDefault(o => o.SelectItemName == "主营菜系");
            if (null == model_selectItem_caixiOne)
            {
                model_selectItem_caixiOne = selectItems.FirstOrDefault(o => o.SelectItemName == "主营项目");
            }
            model_selectItem_caixiTwo = selectItems.FirstOrDefault(o => o.SelectItemName == "选择菜系");
            if (null == model_selectItem_caixiTwo)
            {
                model_selectItem_caixiTwo = selectItems.FirstOrDefault(o => o.SelectItemName == "选择类别");
            }
            model_selectItem_areaOne = selectItems.FirstOrDefault(o => o.SelectItemName == "选择城市");
            model_selectItem_areaTwo = selectItems.FirstOrDefault(o => o.SelectItemName == "选择地区");
            model_selectItem_sceneOne = selectItems.FirstOrDefault(o => o.SelectItemName == "环境氛围");

            var oldSelectItems = bll_selectDetail.GetModelList(" Business_Id= '" + businessId + "'");
            foreach (var item in oldSelectItems)
            {
                bll_selectDetail.Delete(item.Id);
            }

            model_selectDetail_caixiOne = new businessselectdetail()
            {
                Id = Guid.NewGuid().ToString(),
                Business_Id = businessId,
                Item_Id = model_selectItem_caixiOne.Id,
                SelectItem_Id = caixiOneId,
            };
            bll_selectDetail.Add(model_selectDetail_caixiOne);

            model_selectDetail_caixiTwo = new businessselectdetail()
            {
                Id = Guid.NewGuid().ToString(),
                Business_Id = businessId,
                Item_Id = model_selectItem_caixiTwo.Id,
                SelectItem_Id = caixiTwoId,
            };
            bll_selectDetail.Add(model_selectDetail_caixiTwo);

            model_selectDetail_areaOne = new businessselectdetail()
            {
                Id = Guid.NewGuid().ToString(),
                Business_Id = businessId,
                Item_Id = model_selectItem_areaOne.Id,
                SelectItem_Id = areaOneId,
            };
            bll_selectDetail.Add(model_selectDetail_areaOne);

            model_selectDetail_areaTwo = new businessselectdetail()
            {
                Id = Guid.NewGuid().ToString(),
                Business_Id = businessId,
                Item_Id = model_selectItem_areaTwo.Id,
                SelectItem_Id = areaTwoId,
            };
            bll_selectDetail.Add(model_selectDetail_areaTwo);

            if (null != model_selectItem_sceneOne)
            {
                model_selectDetail_sceneOne = new businessselectdetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    Business_Id = businessId,
                    Item_Id = model_selectItem_sceneOne.Id,
                    SelectItem_Id = scencOneId,
                };
                bll_selectDetail.Add(model_selectDetail_sceneOne);
            }

            return "SUCCESS";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateImgs()
        {
            var context = System.Web.HttpContext.Current;
            var savePath = Server.MapPath("~/Uploads/");

            //验证文件夹是否存在
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var postedFile = context.Request.Files["file"];
            var response = context.Response;
            var fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(postedFile.FileName));
            var image = System.Drawing.Image.FromStream(postedFile.InputStream);

            image.Save(Path.Combine(savePath, fileName), image.RawFormat);

            IMeiWei.BLL.businessimgs bll_image = new IMeiWei.BLL.businessimgs();
            IMeiWei.Model.businessimgs model_image = new IMeiWei.Model.businessimgs();


            var businessId = context.Request["businessId"];
            var order = context.Request["order"];

            var old_models = bll_image.GetModelList("Business_Id = '" + businessId + "' and OrderNo = '" + order + "'");

            if (old_models == null || old_models.Count == 0)
            {
                model_image.Business_Id = businessId;
                model_image.OrderNo = Convert.ToInt32(order);
                model_image.picURL = fileName;
                model_image.Status = "ACTIVE";

                bll_image.Add(model_image);
                response.Write(fileName);
            }
            else
            {
                var old_model = old_models.FirstOrDefault();
                old_model.picURL = fileName;
                bll_image.Update(old_model);

                response.Write(old_model.picURL);
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetBusiness()
        {
            IMeiWei.BLL.business bll_business = new IMeiWei.BLL.business();

            var models = bll_business.GetModelList("1 = 1");
            return JsonHelper.GenJSSerializer(Context).Serialize(models);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetUserInfo()
        {
            var js = GenJSSerializer();
            IMeiWei.BLL.user bll_user = new IMeiWei.BLL.user();

            var models = bll_user.GetModelList("1=1");

            return js.Serialize(models);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GenTicketInfo(int num, int amount, DateTime limitTime)
        {
            IMeiWei.BLL.ticket bll_tickect = new IMeiWei.BLL.ticket();

            IMeiWei.Model.ticket model_ticket = new IMeiWei.Model.ticket();

            model_ticket.TicketReferenceId = Guid.NewGuid().ToString();
            model_ticket.TicketNumber = num;
            model_ticket.TicketAmount = amount;
            model_ticket.LimitTime = limitTime;
            model_ticket.CreateTime = DateTime.Now;
            model_ticket.CreateBy = "admin";
            bll_tickect.Add(model_ticket);

            IMeiWei.BLL.ticketinfo bll_ticketInfo = new IMeiWei.BLL.ticketinfo();
            for (int i = 1; i <= num; i++)
            {
                IMeiWei.Model.ticketinfo model_ticketinfo = new IMeiWei.Model.ticketinfo();
                model_ticketinfo.Status = "0";
                model_ticketinfo.TicketReferenceId = model_ticket.TicketReferenceId;
                model_ticketinfo.Code = DateTime.Now.ToString("yyyyMMddHHmmss") + GenCode(i.ToString());
                bll_ticketInfo.Add(model_ticketinfo);
            }
            return "SUCCESS";
        }


        #region App Categrey

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LoadBusinessTypes()
        {
            IMeiWei.BLL.businesstype bll_businessType = new IMeiWei.BLL.businesstype();
            var models = bll_businessType.GetModelList(" 1 = 1");
            var js = GenJSSerializer();
            return js.Serialize(models);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LoadClassOneItems(string businessTypeId)
        {
            IMeiWei.BLL.businesstypeclassone bll_classOne = new IMeiWei.BLL.businesstypeclassone();
            var models = bll_classOne.GetModelList("BusinessType_Id = '" + businessTypeId + "'");
            var js = GenJSSerializer();
            return js.Serialize(models);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveBusinessClassOne(string businessTypeId, string id, string classOneItem)
        {
            IMeiWei.BLL.businesstypeclassone bll_classOne = new IMeiWei.BLL.businesstypeclassone();
            IMeiWei.Model.businesstypeclassone model_classOne = new businesstypeclassone();

            model_classOne.BusinessType_Id = businessTypeId;
            model_classOne.ClassOneItem = classOneItem;
            model_classOne.ClassOneItemStatus = "ACTIVE";
            model_classOne.CreateBy = "SYSTEM_SET";
            model_classOne.CreateTime = DateTime.Now;
            model_classOne.ModifyBy = "SYSTEM_SET";
            model_classOne.ModifyTime = DateTime.Now;
            model_classOne.Id = id;

            bll_classOne.Add(model_classOne);
            var js = GenJSSerializer();

            return js.Serialize("SUCCESS");
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteBusinessClassOne(string id)
        {
            IMeiWei.BLL.businesstypeclassone bll_classOne = new IMeiWei.BLL.businesstypeclassone();

            bll_classOne.Delete(id);

            var js = GenJSSerializer();

            return js.Serialize("SUCCESS");
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LoadClassTwoItems(string businesstypeclassOneId)
        {
            IMeiWei.BLL.businesstypeclasstwo bll_classTwo = new IMeiWei.BLL.businesstypeclasstwo();
            var models = bll_classTwo.GetModelList("BusinessTypeClassOne_Id = '" + businesstypeclassOneId + "'");
            var js = GenJSSerializer();
            return js.Serialize(models);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveBusinessClassTwo(string id, string businesstypeclassOneId, string businessTypeClassTwoName)
        {
            IMeiWei.BLL.businesstypeclasstwo bll_classTwo = new IMeiWei.BLL.businesstypeclasstwo();
            IMeiWei.Model.businesstypeclasstwo model_classTwo = new businesstypeclasstwo();

            model_classTwo.BusinessTypeClassOne_Id = businesstypeclassOneId;
            model_classTwo.BusinessTypeClassTwoName = businessTypeClassTwoName;
            model_classTwo.BusinessTypeClassTwoStatus = "ACTIVE";
            model_classTwo.CreateBy = "SYSTEM_SET";
            model_classTwo.CreateTime = DateTime.Now;
            model_classTwo.ModifyBy = "SYSTEM_SET";
            model_classTwo.ModifyTime = DateTime.Now;
            model_classTwo.Id = id;

            bll_classTwo.Add(model_classTwo);
            var js = GenJSSerializer();

            return js.Serialize("SUCCESS");
        }

        #endregion

        #region Article

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateArticleImgs()
        {
            var context = System.Web.HttpContext.Current;
            var savePath = Server.MapPath("~/Uploads/");

            //验证文件夹是否存在
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var postedFile = context.Request.Files["file"];
            var response = context.Response;
            var fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(postedFile.FileName));
            var image = System.Drawing.Image.FromStream(postedFile.InputStream);

            image.Save(Path.Combine(savePath, fileName), image.RawFormat);

            IMeiWei.BLL.articleimg bll_image = new IMeiWei.BLL.articleimg();
            IMeiWei.Model.articleimg model_image = new IMeiWei.Model.articleimg();


            var articleId = context.Request["articleId"];

            var old_model = bll_image.GetModelList("Article_Id = '" + articleId + "'");

            if (old_model == null || old_model.Count == 0)
            {
                model_image.Article_Id = articleId;
                model_image.Id = Guid.NewGuid().ToString();
                model_image.ArticleImgName = fileName;
                model_image.ArticleImgStatus = "ACTIVE";
                model_image.ModifyBy = "admin";
                model_image.ModifyTime = DateTime.Now;

                bll_image.Add(model_image);
                response.Write(fileName);
            }
            else
            {
                response.Write(old_model.FirstOrDefault().ArticleImgName);
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateArticle(string id, string articleTitle, string articleLink, string readNo, string shareNo)
        {
            IMeiWei.BLL.article bll_article = new IMeiWei.BLL.article();
            IMeiWei.Model.article model_article = new IMeiWei.Model.article();

            var models = bll_article.GetModelList("Id = '" + id + "'");

            if (null != models && models.Count > 0)
            {
                model_article.Id = id;
                model_article.CreateBy = "admin";
                model_article.ArticleLink = articleLink;
                model_article.ArticleStatus = "ACTIVE";
                model_article.ArticleTitle = articleTitle;
                model_article.CreateTime = DateTime.Now;
                model_article.PublishTime = DateTime.Now;
                model_article.ReadNo = Convert.ToInt32(readNo);
                model_article.ShareNo = Convert.ToInt32(shareNo);

                bll_article.Add(model_article);
            }

            return "SUCCESS";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetLatestNews()
        {
            IMeiWei.BLL.articleimg bll_articleimg = new IMeiWei.BLL.articleimg();
            IMeiWei.BLL.article bll_article = new IMeiWei.BLL.article();
            var articleImgModels = bll_articleimg.GetModelList(" 1=1 ");
            var articles = bll_article.GetModelList(" 1=1 ");

            List<Models.ImageInfo> results = new List<Models.ImageInfo>();

            foreach (var item in articleImgModels)
            {
                Models.ImageInfo imageinfo = new Models.ImageInfo();
                var article = articles.FirstOrDefault(o => o.Id == item.Article_Id);

                imageinfo.UrlName = item.ArticleImgName;
                if (null != article)
                {
                    imageinfo.ShareNumber = article.ShareNo;
                    imageinfo.ShareNumber = article.ReadNo;
                }
                results.Add(imageinfo);
            }

            return GenJSSerializer().Serialize(results);

        }

        #endregion

        #region Ad Settings

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveAdImgs()
        {
            var context = System.Web.HttpContext.Current;
            var savePath = Server.MapPath("~/Uploads/");

            //验证文件夹是否存在
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var postedFile = context.Request.Files["file"];
            var response = context.Response;
            var fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(postedFile.FileName));
            var image = System.Drawing.Image.FromStream(postedFile.InputStream);

            image.Save(Path.Combine(savePath, fileName), image.RawFormat);

            IMeiWei.BLL.adimgs bll_image = new IMeiWei.BLL.adimgs();
            IMeiWei.Model.adimgs model_image = new IMeiWei.Model.adimgs();


            var adSetting_Id = context.Request["adSetting_Id"];

            var old_model = bll_image.GetModelList("AdSettings_Id = '" + adSetting_Id + "'");

            if (old_model == null || old_model.Count == 0)
            {
                model_image.AdSettings_Id = adSetting_Id;
                model_image.Id = Guid.NewGuid().ToString();
                model_image.UrlName = fileName;
                model_image.ImgStatus = "ACTIVE";

                bll_image.Add(model_image);
                response.Write(fileName);
            }
            else
            {
                response.Write(old_model.FirstOrDefault().UrlName);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateAdSettings(string id, string flag, string adType, string businessId)
        {
            IMeiWei.BLL.adsetting bll_adsetting = new IMeiWei.BLL.adsetting();
            IMeiWei.Model.adsetting model_adsetting = new IMeiWei.Model.adsetting();

            var models = bll_adsetting.GetModelList("Id = '" + id + "'");

            if (null == models || models.Count == 0)
            {
                model_adsetting.Id = id;

                model_adsetting.AdStartFlag = flag;
                model_adsetting.AdStatus = "ACTIVE";
                model_adsetting.AdType = adType;
                model_adsetting.BusinessId = businessId;
                model_adsetting.LimitTime = DateTime.Now;
                model_adsetting.CreateBy = "admin";
                model_adsetting.CreateTime = DateTime.Now;
                model_adsetting.ModifyBy = "admin";
                model_adsetting.ModifyTime = DateTime.Now;

                bll_adsetting.Add(model_adsetting);
            }

            return "SUCCESS";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LoadStartUpImgs()
        {
            List<Models.ImageInfo> results = new List<Models.ImageInfo>();
            IMeiWei.BLL.adsetting bll_adsetting = new IMeiWei.BLL.adsetting();
            IMeiWei.BLL.adimgs bll_adimgs = new IMeiWei.BLL.adimgs();

            var imgs = bll_adimgs.GetModelList("1 = 1");
            var adSettings = bll_adsetting.GetModelList("1 = 1");

            foreach (var item in imgs)
            {
                Models.ImageInfo imageinfo = new Models.ImageInfo();
                var article = adSettings.FirstOrDefault(o => o.Id == item.AdSettings_Id);

                imageinfo.UrlName = item.UrlName;
                imageinfo.IsBusinessId = true;

                results.Add(imageinfo);
            }


            return GenJSSerializer().Serialize(results);
        }

        #endregion

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetId(string idType)
        {
            IMeiWei.BLL.idfactory bll_idFactory = new IMeiWei.BLL.idfactory();
            IMeiWei.Model.idfactory model_idFactory = new IMeiWei.Model.idfactory();
            var model = bll_idFactory.GetModelList(" IdType = '" + idType + "'").FirstOrDefault();

            model_idFactory.CurrentBusinessId = model.CurrentBusinessId;

            model.CurrentBusinessId++;
            bll_idFactory.Update(model);

            return GenJSSerializer().Serialize(model_idFactory);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LoadSenceClassificationes()
        {
            IMeiWei.BLL.sceneclassification bll_sence = new IMeiWei.BLL.sceneclassification();

            var models = bll_sence.GetModelList(" 1=1");

            return GenJSSerializer().Serialize(models);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveSenceClassificationes(string id, string senceName)
        {
            IMeiWei.BLL.sceneclassification bll_sence = new IMeiWei.BLL.sceneclassification();
            IMeiWei.Model.sceneclassification modle_sence = new IMeiWei.Model.sceneclassification();

            modle_sence.Id = id;
            modle_sence.SceneName = senceName;
            modle_sence.SceneStatus = "ACTIVE";
            modle_sence.CreateBy = "SYSTEM_SET";
            modle_sence.CreateTime = DateTime.Now;
            modle_sence.ModifyBy = "SYSTEM_SET";
            modle_sence.ModifyTime = DateTime.Now;

            bll_sence.Add(modle_sence);

            return GenJSSerializer().Serialize("SUCCESS");

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LoadAreaTwoes(string areaOneId)
        {
            IMeiWei.BLL.areatwo bll_area = new IMeiWei.BLL.areatwo();

            var models = bll_area.GetModelList(" AreaOne_Id = '" + areaOneId + "'");

            return GenJSSerializer().Serialize(models);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveAreaTwoes(string id, string areaOne_Id, string areaTwoName)
        {
            IMeiWei.BLL.areatwo bll_areaTwo = new IMeiWei.BLL.areatwo();
            IMeiWei.Model.areatwo modle_areaTwo = new IMeiWei.Model.areatwo();

            modle_areaTwo.Id = id;
            modle_areaTwo.AreaTwo = areaTwoName;
            modle_areaTwo.AreaOne_Id = areaOne_Id;
            modle_areaTwo.AreaTwoStatus = "ACTIVE";
            modle_areaTwo.CreateBy = "SYSTEM_SET";
            modle_areaTwo.CreateTime = DateTime.Now;
            modle_areaTwo.MofifyBy = "SYSTEM_SET";
            modle_areaTwo.ModifyTime = DateTime.Now;

            bll_areaTwo.Add(modle_areaTwo);

            return GenJSSerializer().Serialize("SUCCESS");
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetServiceSetting()
        {
            IMeiWei.BLL.servicesettings bll_service = new IMeiWei.BLL.servicesettings();

            var model = bll_service.GetModelList(" RecordStatus = 'ACTIVE'").FirstOrDefault();

            if (null == model)
            {
                model = new servicesettings();
            }

            return GenJSSerializer().Serialize(model);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveServiceSetting(decimal VIPAmount, int VIPMouth, decimal AllelicAmount, int AlielicNumber)
        {
            IMeiWei.BLL.servicesettings bll_service = new IMeiWei.BLL.servicesettings();

            var model = bll_service.GetModelList(" RecordStatus = 'ACTIVE'").FirstOrDefault();

            if (null != model)
            {
                model.RecordStatus = "INACTIVE";
                bll_service.Update(model);
            }

            var newModel = new IMeiWei.Model.servicesettings();

            newModel.Id = Guid.NewGuid().ToString();
            newModel.RecordStatus = "ACTIVE";
            newModel.AlielicNumber = AlielicNumber;
            newModel.AllelicAmount = AllelicAmount;
            newModel.CreateBy = "SYSTEM_SET";
            newModel.CreateTime = DateTime.Now;
            newModel.VIPAmount = VIPAmount;
            newModel.VIPMouth = VIPMouth;

            bll_service.Add(newModel);

            return GenJSSerializer().Serialize(newModel);
        }


        /// <summary>
        /// 返回拼接字符
        /// </summary>
        /// <param name="numStr"></param>
        /// <returns></returns>
        private string GenCode(string numStr)
        {
            switch (numStr.Length)
            {
                case 1:
                    return "0000" + numStr;
                case 2:
                    return "000" + numStr;
                case 3:
                    return "00" + numStr;
                case 4:
                    return "0" + numStr;
                case 5:
                    return numStr;
                default:
                    return numStr;
            }

        }

        /// <summary>
        /// 构造Json  序列化
        /// </summary>
        /// <returns></returns>
        private JavaScriptSerializer GenJSSerializer()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            return js;
        }

        /// <summary>
        /// 根据DataTable生成Json
        /// </summary>
        /// <param name='table'> datatable</param>
        /// <returns> json</returns>
        private string DataTableToJson(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.Append('[');

            string[] columnName = new string[table.Columns.Count];//列名数组
            for (int i = 0; i < table.Columns.Count; i++)
            {
                columnName[i] = table.Columns[i].ColumnName.ToLower();//列名小写
            }
            //拼接列
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sb.Append('{');
                for (int j = 0; j < columnName.Length; j++)
                {
                    sb.Append('"' + columnName[j] + '"' + ":" + '"' + table.Rows[i][j].ToString() + '"');
                    if (j < columnName.Length - 1)
                    {
                        sb.Append(',');
                    }
                }
                sb.Append('}');
                if (i < table.Rows.Count - 1)
                {
                    sb.Append(',');
                }
            }
            sb.Append(']');

            table = null;
            return sb.ToString();
        }

    }

}

