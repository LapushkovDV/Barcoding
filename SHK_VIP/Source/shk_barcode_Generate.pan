#declare windowevent(WindowName, objmarker,ImportFromName)
WindowEvent #WindowName;
CmInit: {
   SetWindowTitle(#WindowName,'Выбор '+tTypeObj_select.name);
   #objmarker.UnselectAll;
   if #ImportFromName <> '' then #objmarker.ImportFromName(#ImportFromName)
}
cmDefault: {
  if (#objmarker.Count=0) then #objmarker.Mark;
  var _tmpMarker : tpTr = InitMarker( co_MarkerName, 8, 100, 10 );
  ClearMarker(_tmpMarker);
   #objmarker.ExportTo(_tmpMarker);
  DoneMarker( _tmpMarker, co_MarkerName);
  closewindow(#WindowName);
}
end;
#end

Window winKS_SELECT 'Выбор накладных', cyan;
browse brKS_SELECT;
 table KS_SELECT;
  recMarker = pMarkerKS;
  Fields
   KS_SELECT.DesGr  'группа','дескрипторов' : [2] , Protect, NoPickButton;
   KS_SELECT.Descr  'Дескриптор'            : [3] , Protect, nopickbutton;
   ks_Note.sNAME     'Статус'                 : [6] , Protect, NoPickButton;
   KS_SELECT.Nsopr  'Номер'                 : [6] , Protect, NoPickButton;
   KS_SELECT.dSopr  'Дата'                 : [4] , Protect, nopickbutton;
   KS_SELECT.dOpr   'Дата','оприходования' : [10], Protect, nopickbutton;
   KS_SELECT.name   'Примечание'           : [10], Protect, nopickbutton;
   ks_Org.name              'Организация'          : [10], Protect, nopickbutton;
   ks_PodrFrom.kod          'Подр. откуда','код'          : [3], Protect, nopickbutton;
   ks_PodrFrom.name         'Подр. откуда','наименование' : [10], Protect, nopickbutton;
   ks_PodrTo.kod            'Подр. куда','код'          : [3], Protect, nopickbutton;
   ks_PodrTo.name           'Подр. куда','наименование' : [10], Protect, nopickbutton;
   ks_MolFrom.kod           'МОЛ откуда','код'          : [3], Protect, nopickbutton;
   ks_MolFrom.name          'МОЛ откуда','наименование' : [10], Protect, nopickbutton;
   ks_MolTo.kod             'МОЛ куда','код'          : [3], Protect, nopickbutton;
   ks_MolTo.name            'МОЛ куда','наименование' : [10], Protect, nopickbutton;
end;
end;
#windowevent(winKS_SELECT, pMarkerKS,'')

Window winINV_SELECT 'Выбор инвентаризации', cyan;
browse brINV_SELECT;
 table INV_SELECT;
  recMarker = pMarkerINV;
  Fields
  INV_SELECT.desGr  'Группа'    ('Код группы пользователей')            : [ 6], Protect, nopickbutton;
  INV_SELECT.descr  'Дескр.' ('Дескриптор(идентификатор) пользователя') : [ 6], Protect, nopickbutton;
  INV_NOTE.sName    'Статус'                                       : [10], Protect,     nopickbutton;
  INV_SELECT.nInv   'Номер'     ('Номер инвентаризации')           : [10], Protect,      nopickbutton;
  INV_SELECT.dInv   'Дата инв.' ('Дата проведения инвентаризации') : [10], Protect,      nopickbutton;
  inv_podr.kod      'Склад код'     ('код склада'           )      : [5], Protect,       nopickbutton;
  inv_podr.Name     'Склад'     ('Наименование склада'           ) : [40], Protect,      nopickbutton;
  inv_mol.kod       'МОЛ код'       ('Материально ответственное лицо код') : [5], Protect,nopickbutton;
  inv_mol.name      'МОЛ'       ('Материально ответственное лицо') : [30], Protect,       nopickbutton;
end;
end;
#windowevent(winINV_SELECT, pMarkerINV,'')

Window winKATBOX_SELECT 'Выбор ячеек', cyan;
browse brKATBOX_SELECT;
 table KATBOX_SELECT;
  recMarker = pMarkerBox;
 Fields
  KATBOX_SELECT.Name 'Наименование ячейки' ('Наименование ячейки', , ) : [20], Protect, nopickbutton;
  KATBOX_SELECT.KOD  'Код ячейки' ('Код ячейки', , ) : [10], Protect, nopickbutton;
  Box_podr.kod       'Подразделение','код'           : [10], Protect, nopickbutton;
  Box_podr.name      'Подразделение','наименование'  : [20], Protect, nopickbutton;
  KATBOX_SELECT.volume      'Объем ячейки'                  : [10.3, '\3p[|-]6`666`666`666`666.888'], Protect, nopickbutton;
  TekBox.volume      'Заполнено объема'              : [10.3, '\3p[|-]6`666`666`666`666.888'], Protect, nopickbutton;
  Box_mc.barkod      'Зарезервирована за МЦ','код'      : [10], Protect, nopickbutton;
  Box_mc.Name        'Зарезервирована за МЦ','наименование'      : [20], Protect, nopickbutton;
end;
end;
#windowevent(winKATBOX_SELECT, pMarkerBox,co_MarkerNameExportSaldoBox)

Window winAddNewBarcodes 'Формирование новых штрихкодов' ;
Screen scrAddNewBarcodes 'Формирование новых штрихкодов';
Fields
 __wTable         : Protect, PickButton;
 //__TidkGal        : Protect, PickButton;
 tTypeObj_select.name : Protect, PickButton;
 __SelectedObject : Protect, pickButton;
Buttons
 cmValue1,,,;
 cmCancel,,,;
<<'Формирование новых штрихкодов'

      Таблица.@@@@@@@@@@@@@@@@@@@@@@@@
Тип документа.@@@@@@@@@@@@@@@@@@@@@@@@

    Объекты  .@@@@@@@@@@@@@@@@@@@@@@@@

  <. Создать .>  <. Отмена.>
>>
end;
end;

windowevent winAddNewBarcodes ;
  cmInit: {
     var _tmpMarker : tpTr = InitMarker( co_MarkerName, 8, 100, 10 );
     ClearMarker( _tmpMarker );
     DoneMarker( _tmpMarker, co_MarkerName);
     set __TidkGal   := 0;
     set __SelectedObject := '';
 }
 cmDelOnProtect: {
   case curfield of #__SelectedObject : __SelectedObject  := ''; end;
 }
 cmpick: {
   case curfield of
    #__TidkGal, #__wTable,  #tTypeObj_select.name: {
       if __SelectedObject <> '' {
         Message('Сначала удалите выбранные объекты', error);
         stop; abort; exit;
       }
      var _iSHK_TEMPLATES : SHK_TEMPLATES new;
      set __TidkGal := _iSHK_TEMPLATES.SelectVIDDOC;
    }
    #__SelectedObject: {

        if __wTable = 0 and __TidkGal = 0 then  {
           Message('Выберите сначала тип документа', error);
           stop; abort; exit;
         }

      set __SelectedObject := SelectObjects;
    }
   end;
 }
 cmValue1: {
   if __SelectedObject = '' then {
     Message('Не все параметры указаны', error);
     stop; abort; exit;
   }
  closewindowex(winAddNewBarcodes, cmDefault);
 }
end;

Window winSelectTSDARMUSER 'Выбор', doAccept;
//---------------------------------------------
Browse brSelectTSDARMUSER (,,Sci1Esc);
  Table SHK_Browse;
  fields {Font={Color=if(not SHK_Browse.IsActive,ColorGray,0)}};
   SHK_Browse.ID          'Код' ('идентификатор ') : [ 6], Protect, nopickbutton;
   SHK_Browse.name        'Наименование' ('Наименование') : [ 6], Protect, nopickbutton;
   SHK_Browse.Description 'Описание'('описание') : [ 10], Protect, nopickbutton;
   SHK_Browse.IsActive    'Активный'('признак активности') : [ 2], Protect, checkBox;
 end;
end;

Function selectTSDARMUSER(_TSDARMUSER: word; _curvalue : comp): comp;
{
  set _pTypeTSDARMUSER := _TSDARMUSER;
  result := _curvalue;
  if runWindowModal(winSelectTSDARMUSER) = cmDefault then {
      result := SHK_Browse.nrec;
    }
}

Window winshk_barcodeLink 'Просмотр/создание штрихкодов' ;
//---------------------------------------------
Browse brSshk_barcodeLink (,,Sci17Esc);
Table shk_barcodeLink;
  recMarker = pMarkerBarCodeLink;
Fields //{font = {color= if(SHKObjSP_HEAD.WISACTIVE = 0,colorsysgray,0)}};
// shk_barcodeLink.wTable     'Код таблицы'                    ('Код таблицы'                  ) : [3] ,Protect, NoDel;
 x$files.xf$name            'Таблица'                        ('Таблица наименование'                  ) : [3] ,Protect, NoDel;
// shk_barcodeLink.tiDkGal    'Системный код типа документа'   ('Системный код типа документа' ) : [3] ,Protect, NoDel;
 tTypeObj.name               'Тип документа'                  ('тип документа' ) : [3] ,Protect, NoDel;
// shk_barcodeLink.cRec       'Ссылка на объект'               ('Ссылка на объект'             ) : [4] ,Protect, NoDel;
 ObjectString               'Объект'                         ('Ссылка на объект'             ) : [4] ,Protect, NoDel;
 shk_barcodeLink.barcode    'Штрих код документа'            ('Штрих код документа'          ) : [5] ,Protect, NoDel;
 shk_barcodeLink.type       'Тип ШК: 0 - свой, 1 - внешний'  ('Тип ШК: 0 - свой, 1 - внешний') : [3] ,Protect, NoDel;
 [SHK_TSD_name] SHK_TSD.name + '(' +SHK_TSD.id+ ')'  'ТСД'            ('Целевой ТСД для передачи посредством USB') : [5] ,Protect, PickButton;
 [SHK_ARM_name] SHK_ARM.name + '(' +SHK_ARM.id+ ')' 'АРМ'            ('Целевой АРМ для передачи посредством USB') : [5] ,Protect, PickButton;
 [SHK_USER_name] SHK_USER.name + '(' +SHK_USER.id+ ')' 'Пользователь'   ('Целевой пользователь для передачи посредством USB') : [5] ,Protect, PickButton;

end;//Browse brNormPercent
end;

windowevent winshk_barcodeLink;
 cmPick: {
   case curfield of
   #SHK_TSD_name  : shk_barcodeLink.cTSD  := selectTSDARMUSER(1,shk_barcodeLink.cTSD);
   #SHK_ARM_name  : shk_barcodeLink.cARM  := selectTSDARMUSER(2,shk_barcodeLink.cARM );
   #SHK_USER_name : shk_barcodeLink.cUser := selectTSDARMUSER(3,shk_barcodeLink.cUser);
  end;
  update current shk_barcodeLink;
  rereadrecord;
 }
 cminsert: {
  AddNewBarcodes;
  rereadrecord;
 }
 cmdelonprotect:{
   case curfield of
   #SHK_TSD_name  : shk_barcodeLink.cTSD  := 0h;
   #SHK_ARM_name  : shk_barcodeLink.cARM  := 0h;
   #SHK_USER_name : shk_barcodeLink.cUser := 0h;
  end;
  update current shk_barcodeLink;
  rereadrecord;

 }
 cmPrintDoc: {
//        message(pMarkerBarCodeLink.count);
        if pMarkerBarCodeLink.count = 0
         {
           var _ishk_barcode_Print : shk_barcode_Print;
            _ishk_barcode_Print.PrintByBarCode(shk_barcodeLink.barcode,shk_barcodeLink.type,'');
         }
         else {

             var _tmpMarker : tpTr = InitMarker( 'PrintSeveralBarcodes', 8, 100, 10 );
              ClearMarker(_tmpMarker);
               pMarkerBarCodeLink.ExportTo(_tmpMarker);
              DoneMarker( _tmpMarker, 'PrintSeveralBarcodes');
              pMarkerBarCodeLink.UnselectAll;
            var _TpTunePrintGen : TpTunePrintGen;
              clearadvrecord(_TpTunePrintGen);
              rescanpanel(tnshk_barcodeLink);
             var ishk_barcode_Print :shk_barcode_Print;
               ishk_barcode_Print.PrintByMarkerBarcodeLink('PrintSeveralBarcodes', _TpTunePrintGen, '');
           }

    }
 cmValue1 : {
   if not isvalidall(tnshk_barcodeLink) then exit;
   var _ishk_barcode_Print : shk_barcode_Print;
    _ishk_barcode_Print.TunePrint(shk_barcodeLink.wTable, shk_barcodeLink.tidkGal);

 }
 cmValue2 : {
   if not isvalidall(tnshk_barcodeLink) then exit;
    _iShk_BarcodeFunc.show_doc(shk_barcodeLink.TidkGal, shk_barcodeLink.crec);

 }

 cmHotKeys : {
    PutHotCommand(runMenu('mnu_shk_barcode_Generate'));
 }

end;
