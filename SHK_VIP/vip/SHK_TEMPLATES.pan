#declare tableeventtable (table)
TableEvent table #table;
cmSetDefault: {
  #table.WISACTIVE   := 1;
  #table.BROWSE_CARD := true;
}
cmInsertRecord:
{
  Insert Current #table;
}
cmUpdateRecord:
{
  Update Current #table;
}
cmDeleteRecord:
{
 if GetSpecFieldIDName = SHKObjSP_SP.SYSNAMETBL + '_'+SHKObjSP_SP.SYSNAME then
 {
   stop;abort;exit
 }
 if message('Удаление может изменить существующие документы'
   +chr(13)+'Продолжить?',YesNo)<>cmYes
  { abort; exit;
  }
  delete Current #table;
}
cmcheckfield: {
   Case CurField of
       #SHKObjSP_BTN.isSynchAction: {
       if SHKObjSP_BTN.isSynchAction = False {
         set SHKObjSP_BTN.isDeleteDoc := False;
         update current SHKObjSP_BTN;
       }
       SetSkipFields;
     }
   end;
   updatetable;
}
end; //TableEvent table #table
#end

#declare colorneed (FldCondition)
{Font={BackColor=if(#FldCondition,ColorNeed,0)}}
#end
window wintActions 'Выбор действия', cyan;
browse brwintActions;
 table tActions;
  Fields
   tActions.NAME         'SYSNAME'  : [7], Protect, NoPickButton;
   tActions.DESCRIPTION  'Описание' :[15], Protect, NoPickButton;
end;
end;
windowevent wintActions ;
 cmdefault: {
   closewindowex(wintActions, cmDefault)
 }
end;

window wintSelGroup 'Выбор Группы', cyan;
browse brwinGroup;
 table SHKObj_group;
  Fields
  SHKObj_group.code 'Код'  : [1], Protect, NoPickButton;
  SHKObj_group.Name 'Наименование'  : [7], Protect, NoPickButton;
  SHKObj_group.Description 'Описание'  : [7], Protect, NoPickButton;
end;
end;
windowevent wintSelGroup ;
cmInit:{
  if getfirst SHKObj_group = tsOK {}
  rereadrecord;
}
 cmdefault: {
   closewindowex(wintSelGroup, cmDefault)
 }
end;


window winSelAllowStatus 'Разрешенные статусы', cyan;
browse brwinSelAllowStatus (,,Sci178Esc);;
 table SHKObjSP_AllowStatus;
  Fields
   KN_AllowStatus.code   'Код'                   : [4], Protect, NoPickButton;
   KN_AllowStatus.sName  'Наименование короткое' : [6], Protect, nopickbutton, {Font={BackColor=if(SHKObj.Name='',ColorNeed,0)}};
   KN_AllowStatus.Name   'Наименование'          : [10], Protect, nopickbutton, {Font={BackColor=if(SHKObj.Name='',ColorNeed,0)}};
end;
end;
TableEvent table SHKObjSP_AllowStatus;
cmSetDefault:{
   If(RunInterface('L_Dogovor::GetSomKatNotes', SHKObj.VIDDOC, 0, 0, true, 0h, IGetSomKatNotes(NullRef)) = CmDefault) {
       var MarkerVidD : longint = InitMarker('MKatNotes', 8, 100, 10);
       var _cnt : longint = GetMarkerCount(MarkerVidD);
       var _i : word = 0;
        for( _i := 0; _i < _cnt; _i++)
         {
          var _cNote : comp = 0h;

            if (not GetMarker(MarkerVidD, _i, _cNote)) then continue;

            if(getfirst SHKObjSP_AllowStatus where ((SHKObj.nrec == SHKObjSP_AllowStatus.cSHK_TMPLT_OBJ
                                       and coVIDFIELD_AlloStatus == SHKObjSP_AllowStatus.VIDFIELD))
                                                and SHKObjSP_AllowStatus.ADDCOMP = _cNote
              ) <> tsOK
              {
                //var _npp : word =0;
                mylog('===================');
                mylog('SHKObj.nrec           ' + string(SHKObj.nrec,0,0) );
                mylog('coVIDFIELD_AlloStatus ' + string(coVIDFIELD_AlloStatus) );
                //mylog('string(SHKObj.nrec)   ' + string(_cNote,0,0) );
                //mylog('string(SHKObj.nrec)   ' + string(_cNote,0,0) );
                mylog('_cNote                ' + string(_cNote,0,0) );
                 insert SHKObjSP_AllowStatus set
                    SHKObjSP_AllowStatus.cSHK_TMPLT_OBJ := SHKObj.nrec
                  , SHKObjSP_AllowStatus.VIDFIELD       := coVIDFIELD_AlloStatus
                  , SHKObjSP_AllowStatus.SYSNAMETBL     := string(_cNote) // там индекс уникльный
                  , SHKObjSP_AllowStatus.NAME           := string(_cNote) // там индекс уникльный
                  , SHKObjSP_AllowStatus.ADDCOMP        := _cNote
                 ;
              }

         }
       clearmarker(MarkerVidD);
       donemarker(MarkerVidD,'MKatNotes');
    }
   else {
    stop; abort;exit;
  }
  if getlast SHKObjSP_AllowStatus = tsOK {}
  rereadrecord;
}
cmDeleteRecord:
{
 if message('Удалить запись?',YesNo)<>cmYes
  { abort; exit;
  }
  delete Current SHKObjSP_AllowStatus;
}
end; //TableEvent table #table



window winSetBatchTerms 'Условия для пакетной выгрузки';
browse brwinSetBatchTerms (,,Sci178Esc);
 show at (,,40,) ;
 table SHKObjSP_BatchTerms;
  Fields
   SHKObjSP_BatchTerms.ROW    'Строка'          : NoProtect, {Font={BackColor=if(SHKObjSP_BatchTerms.ROW=0,ColorERROR,0)}};
   SHKObjSP_BatchTerms.Name   'Условие'         : NoProtect, {Font={BackColor=if(SHKObjSP_BatchTerms.Name='',ColorNeed,0)}};
end;
browse brwintDescriptionQuery (,,Sci1Esc);
 show at (41,,,) ;
 table tDescriptionQuery;
  Fields
   tDescriptionQuery.npp   '№ п/п'    : Protect;
   tDescriptionQuery.Descr 'Описание' : Protect;
end;
end;
windowevent winSetBatchTerms ;
 cmInit: {
   Fill_fieldArray(SHKObj.viddoc)
 }
end;
TableEvent table SHKObjSP_BatchTerms;
cmsetdefault:{
// SHKObjSP_BatchTerms.name := '// можно использовать #(BatchDate), что автоматом подставит SHKObj.BatchDate "Дата последней пакетной выгрузки"'
}
cmInsertRecord:{
  SHKObjSP_BatchTerms.WISACTIVE  := 1;
  SHKObjSP_BatchTerms.SYSNAMETBL := string(SHKObjSP_BatchTerms.nrec);
  SHKObjSP_BatchTerms.SYSNAME    := string(SHKObjSP_BatchTerms.nrec);
  Insert Current SHKObjSP_BatchTerms;
}
cmUpdateRecord:{
  SHKObjSP_BatchTerms.SYSNAMETBL := string(SHKObjSP_BatchTerms.nrec);
  SHKObjSP_BatchTerms.SYSNAME    := string(SHKObjSP_BatchTerms.nrec);
  Update Current SHKObjSP_BatchTerms;
}
cmDeleteRecord:
{
 if message('Удалить?',YesNo)<>cmYes
  { abort; exit;
  }
  delete Current SHKObjSP_BatchTerms;
}
end; //TableEvent table #table



window winSelObjActions 'Выбор шаблона действия', cyan;
browse brwinSelObjActions;
 table SHKObjSel;
  Fields
  SHKObjSel.code        'Код'             : [3] , Protect, nopickbutton, {Font={BackColor=if(SHKObj.code='',ColorNeed,0)}};
  SHKObjSel.Name        'Наименование'    : [10], Protect, nopickbutton, {Font={BackColor=if(SHKObj.Name='',ColorNeed,0)}};
  SHKObjSel.Description 'Описание'        : [15], Protect, NoPickButton;
  SHKObjSel.Action       'Действие'       : [7],  Protect, NoPickButton;
end;
end;
windowevent winSelObjActions ;
 cmdefault: {
   closewindowex(winSelObjActions, cmDefault)
 }
end;

window wintFields 'Выбор поля таблиц', cyan;
browse brwintFields;
 table tFields;
  Fields
   tFields.SYSNAMETBL 'Таблица'    : [8], Protect, NoPickButton;
   tFields.NAME       'SYSNAME'      :[10], Protect, NoPickButton;
   tFields.TITLE      'Наименование' :[15], Protect, NoPickButton;
   tFields.DATATYPE   'Тип данных'   : [4], Protect, NoPickButton;
end;
end;
windowevent wintFields ;
 cmdefault: {
   closewindowex(wintFields, cmDefault)
 }
end;


window wintTypeObj'Выбор типа объекта', cyan;
browse brwintTypeObj;
 table tTypeObj;
  Fields
   tTypeObj.name    'Наименование' :[10], Protect, NoPickButton;
   tTypeObj.tidkGal 'Тип'          :[3] , Protect, NoPickButton;
end;
end;
windowevent wintTypeObj ;
 cmdefault: {
   closewindowex(wintTypeObj, cmDefault)
 }
end;

Window wnSHK_TMPLT_Edit 'Редактирование параметров объекта ШК' ;
Show at (3,5,120,28);
//---------------------------------------------
Screen ScrSHK_getParameter (,,Sci178Esc);
Show at (,,,3);
Table SHKObj;
Fields
 SHKObj_UP.name      : Protect, PickButton;
 SHKObj.code         : NoProtect            , #colorneed(SHKObj.code='');
 SHKObj.Name         : NoProtect            , #colorneed(SHKObj.Name='');
 SHKObj.Action        ('Уникальный идентификатор (по нему идет сопоставление, что именно запрашивается)'): NoProtect            , #colorneed(SHKObj.Action='');
 'Тип объекта: '+if(ISDOC, 'документ', 'каталог') : skip;
 tTypeObjWin.name    :   Protect, PickButton, #colorneed(not isvalidall(tntTypeObjWin));
 SHKObj.SYSNAMETBL   : Protect, NoPickButton, NoDel, #colorneed(SHKObj.SYSNAMETBL = '');
 SHKObj.SYSNAME      : Protect, NoPickButton, NoDel, #colorneed(SHKObj.SYSNAME = '');
 SHKObj.UserSysBarcode : NoProtect, NoPickButton, NoDel;
 SHKObj.Description  : NoProtect, NoPickButton, NoDel;
 SHKObj.UseBatchLoad : NoProtect, NoPickButton, NoDel;
 SHKObj.WISACTIVE    : [List 0 'Нет','Да'],Protect;
 SHKObj.ALLOWADDROWS : [List 0 'Нет','Да'],Protect;
 SHKObj.BlockRows    : [List 0 'Нет','Да'],Protect, skip;
 AllowStatus         : Protect, PickButton;
 SHKObj.IsMenuPoint   : [List 0 'Нет','Да'],Protect;
 SHKObj.MenuPointName : NoProtect;
 SHKObj.isLogging     : NoProtect;
 SHKObj.BatchDate     : NoProtect;
 NoteAfterBatch.name : Protect, pickbutton;
 SHKObj.BatchDays     : NoProtect;
 SHKObj.BatchMinutes  : NoProtect;

buttons
 cmValue2,,,;
 cmValue1,,,;
<<
 `Группа` .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 `Код` .@@@@@@@ `Наименование`.@@@@@@@@@@@@@@@@@@@@@@@`Идентификатор OBJECTACTION`.@@@@@@@@@@@@@@@@@@@@@@@@@@@  .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 `Тип документа` .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@`Поле идентификации объекта`.@@@@@@@@@@@@@@.@@@@@@@@@@@@@@ [:] - использовать системный ШК     `
 `Описание` .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ [:] - использовать пакетную выгрузку`
 `Использовать в настройках шаблонов` .@@@@@       `Добавлять спецификацию на ТСД`.@@@@                               <.Условия для пакетной выгрузки.>
 `Блокировать позиции после обработки`.@@@@@   `Статусы для редактирования на ТСД`.@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 `Является пунктом меню на ТСД`       .@@@@@     `Наименование пункта меню на ТСД`.@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
  [.] вести лог по действиям данного шаблона ` `Дата последней пакетной выгрузки` .@@@@@@@@@@@  `После пакетной выгрузки изменить статус на` .@@@@@@@@@@@@@@@@   <.TEST EXPORT JSON.>
                     `Дата (дни) для построения в условиях пакетной выгрузки`.@@@@@@@@@ `время (минуты) для построения в условиях пакетной выгрузки`.@@@@@@@@@
>>
end;//Screen ScrSHK_getParameter

//---------------------------------------------
panel pnSHK_TMPLT_Edit_HEAD;
Show at (,4,60,9);
Screen ScrSHK_TMPLT_Edit_HEAD(,,Sci178Esc);
Show at (,4,60,4) Fixed_y;
<<
                           `Поля шапки документа`
>>
end;

//---------------------------------------------
Browse brSHK_TMPLT_Edit_HEAD;
Show at (,5,,);
Table SHKObjSP_HEAD;
Fields {font = {color= if(SHKObjSP_HEAD.WISACTIVE = 0,colorsysgray,0)}};
  SHKObjSP_HEAD.ROW  'Строка'                                                           : [2], NoProtect, NoPickButton, NoDel, #colorneed(SHKObjSP_HEAD.ROW = 0);
//  SHKObjSP_HEAD.Code 'Код' ('Код поля отображения в шапке ') :[5]  , PickButton,NoDel;
  SHKObjSP_HEAD.Name       'Наименование','поля' ('Наименование поля для отображения на ТСД'):[10], NoProtect, NoPickButton, NoDel, #colorneed(SHKObjSP_HEAD.Name = '');
  SHKObjSP_HEAD.SYSNAMETBL 'Сист. имя','таблицы/функции' ('Системное имя таблицы или фукнции'):[10], Protect, NoPickButton, NoDel, #colorneed(SHKObjSP_HEAD.Name = '');
  SHKObjSP_HEAD.SysName    'Сист. имя','поля' ('Системное наименование поля для ()')     : [8],   Protect, PickButton  , NoDel, #colorneed(SHKObjSP_HEAD.SysName = '');
  SHKObjSP_HEAD.SIZE       'Размерность','поля' ('Нименование поля для отображения на ТСД')        : [5], NoProtect, NoPickButton, NoDel, #colorneed(SHKObjSP_HEAD.SIZE = 0);
  SHKObjSP_HEAD.WISACTIVE   'Отображать'                                                  : [3] , [List 0 'Нет','Да'],Protect;
  SHKObjSP_HEAD.ISIdentifier 'Идентификатор' ('Поле является идентификатором') : [3] , [List 0 'Нет','Да'],Protect;
  SHKObjSP_HEAD.nullable   'Обнуляемое','поле'                :[3] , [List 0 'Нет','Да'],Protect;
  SHKObjSP_HEAD.BROWSE_CARD  'Отображать','в карточке ТСД'                       : [3] , [List 0 'Нет','Да'],Protect;

end;//Browse brNormPercent
end;

panel pnSHK_TMPLT_Edit_BTN
Show at (61,4,,9);
Screen ScrSHK_TMPLT_Edit_BTN (,,Sci178Esc);
Show at (61,4,,4) Fixed_y;
<<
                           `Кнопки операций`
>>
end;

//---------------------------------------------
Browse brSHKObjSP_BTN;
Show at (,5,,);
 Table SHKObjSP_BTN;
Fields {font = {color= if(SHKObjSP_BTN.WISACTIVE = 0,colorsysgray,0)}};
  SHKObjSP_BTN.ROW  'Строка'                                                                   : [2], NoProtect, NoPickButton, NoDel, #colorneed(SHKObjSP_BTN.ROW = 0);
//  SHKObjSP_HEAD.Code 'Код' ('Код поля отображения в шапке ') :[5]  , PickButton,NoDel;
  SHKObjSP_BTN.Name    'Наименование','действия' ('Наименование действия для отображения на ТСД'):[10], NoProtect, NoPickButton, NoDel, #colorneed(SHKObjSP_BTN.Name = '');
//  SHKObjSP_BTN.SYSNAMETBL 'Сист. имя таблицы/функции' ('Системное имя таблицы или фукнции'):[10], Protect, NoPickButton, NoDel, #colorneed(SHKObjSP_HEAD.Name = '');
  SHKObjSP_BTN.SysName 'Системное','имя действия' ('Системное наименование действия для ()')     : [8],   Protect,   PickButton, NoDel, #colorneed(SHKObjSP_BTN.SysName = '');
  SHKObjSP_BTN.SIZE    'Длина','кнопки' ('Наименование действия для отображения на ТСД')         : [5], NoProtect, NoPickButton, NoDel, #colorneed(SHKObjSP_BTN.SIZE = 0);
  SHKObjSP_BTN.WISACTIVE 'Отображать'                                                          : [3] , [List 0 'Нет','Да'],Protect;
  SHKObjSP_BTN.isSynchAction 'Тип','запроса' ('Тип запроса от ТСД синхронный или асинхронный')   : [5] , [List 0 'Asynch','Synch'],Protect; //: boolean "True-синхронный запрос,False-Асинхронный"
  SHKObjSP_BTN.CloseDoc   'Закрывать','после нажатия' ('Закрывать документ после выполнения действия по кнопке') : [3] , [List 0 'Нет','Да'],Protect;
  SHKObjSP_BTN.isDeleteDoc 'Удалять с ТСД','после обработки' ('Удалять с ТСД после обработки')    : [3] , [List 0 'Нет','Да'],Protect;
  Btn_StatusName          'Целевое','значение' ('выбор целевого значения, которое будет установлено в данном действии') : [8], Protect, PickButton, NoDel, #colorneed(SHKObjSP_BTN.SysName = 'SETSTATUS' and SHKObjSP_BTN.AddComp = 0);
//  SHKObjSP_BTN.AddComp    'Целевое значение' ('выбор целевого значения, которое будет установлено в данном действии') : [8], NoProtect, NoPickButton, NoDel, #colorneed(SHKObjSP_BTN.SysName = 'SETSTATUS' and SHKObjSP_BTN.AddComp = 0);
end;//Browse brNormPercent
end;




Screen ScrSHKObjSP_SP (,,Sci178Esc);
Show at (,10,,10) Fixed_y;
<<
 Поля спецификации документа на ТСД
>>
end;

//---------------------------------------------
Browse brSHKObjSP_SP;
 Show at (,11,,) ;
Table SHKObjSP_SP;
 fixedLeft (SHKObjSP_SP.NPP, SHKObjSP_SP.NAME);
Fields {font = {color= if(SHKObjSP_SP.WISACTIVE = 0,colorsysgray,0)}};
 SHKObjSP_SP.NPP        '№ колонки'              :[2] , NoProtect, nopickbutton    , #colorneed(word(SHKObjSP_SP.NPP)    = 0);
 SHKObjSP_SP.NAME       'Наименование'           :[10], NoProtect, nopickbutton    , #colorneed(SHKObjSP_SP.NAME   = '');
 SHKObjSP_SP.SYSNAMETBL 'Сист. имя','таблицы/функции' ('Системное имя таблицы или фукнции'):[10], Protect, NoPickButton, NoDel, #colorneed(SHKObjSP_HEAD.Name = '');
 SHKObjSP_SP.SYSNAME    'Сист. имя','поля' :[8] ,  Protect,    pickbutton    , #colorneed(SHKObjSP_SP.SYSNAME= '');
 SHKObjSP_SP.SIZE       'Размер'                 :[5] , NoProtect, nopickbutton    , #colorneed(SHKObjSP_SP.SIZE = 0);
 SHKObjSP_SP.MODIF      'Редакт.'                :[3] , [List 0 'Нет','Да'],Protect;
 SHKObjSP_SP.nullable   'Обнуляемое','поле'                :[3] , [List 0 'Нет','Да'],Protect;
 SHKObjAction.ACTION    'Ссылка на шаблон','объекта ШК'               :[10], Protect, PickButton        ;
// SHKObjSP_SP.ACTION    'Действие'               :[10], Protect, PickButton        ;
 SHKObjSP_SP.WISACTIVE 'Отображать','в спецификации'             :[3] , [List 0 'Нет','Да'],Protect;
 SHKObjSP_SP.BROWSE_CARD  'Отображать','в карточке ТСД' : [3] , [List 0 'Нет','Да'],Protect;

end;//Browse brFields;
end;
#tableeventtable(SHKObjSP_HEAD)
#tableeventtable(SHKObjSP_BTN )
#tableeventtable(SHKObjSP_SP  )

TableEvent table SHKObj;
cmInsertRecord:
{
  if _isGroupCreate then SHKObj.Action := OleGenerateGUID;
   SHKObj.isGroup   := _isGroupCreate;
   SHKObj.WISACTIVE := 1;
  Insert Current SHKObj;
}
cmUpdateRecord:
{ Update Current SHKObj;
  var _SpecFieldIDName : string = GetSpecFieldIDName;
  if _SpecFieldIDName <> ''
  {
    if getfirst SHKObjSP_SP where ((    SHKObj.nrec == SHKObjSP_SP.cSHK_TMPLT_OBJ
                                  and coVIDFIELD_SP ==  SHKObjSP_SP.VIDFIELD (noindex)
                         and extractdelimitedword(_SpecFieldIDName,1,'_') == SHKObjSP_SP.SYSNAMETBL (noindex)
                         and extractdelimitedword(_SpecFieldIDName,2,'_') == SHKObjSP_SP.SYSNAME    (noindex)
                                  )) <> tsOK
              {
                insert SHKObjSP_SP set
                  SHKObjSP_SP.cSHK_TMPLT_OBJ := SHKObj.nrec
                , SHKObjSP_SP.VIDFIELD   := coVIDFIELD_SP
                , SHKObjSP_SP.npp        := '9999'
                , SHKObjSP_SP.NAME       := _SpecFieldIDName
                , SHKObjSP_SP.SYSNAMETBL := extractdelimitedword(_SpecFieldIDName,1,'_')
                , SHKObjSP_SP.SYSNAME    := extractdelimitedword(_SpecFieldIDName,2,'_')
                , SHKObjSP_SP.SIZE       := 1
                , SHKObjSP_SP.MODIF      := false
                , SHKObjSP_SP.WISACTIVE  := 0
                ;
             }
  }

}
cmDeleteRecord:
{

  if getfirst SHKObjSel where ((SHKObj.nrec == SHKObjSel.cGroup)) = tsOK {
      message('В группе подчиненные элементы.'+
       +chr(13)+'Удаление запрещено',error);
       abort; exit;
  }

  if message('Удаление может изменить существующие документы'
   +chr(13)+'Продолжить?',YesNo)<>cmYes
  { abort; exit;
  }
  _loop SHKObjSP_Del where ((SHKObj.nrec == SHKObjSP_Del.cSHK_TMPLT_OBJ))
  {
   delete current SHKObjSP_Del;
  }
  delete Current SHKObj;
}
end; //TableEvent table #table

windowevent wnSHK_TMPLT_Edit;
cmValue1:{
  var iSHK_InOut : SHK_InOut;
  iSHK_InOut.TestExportJSON(SHKObj.nrec)
}
cmValue2:{
 RunWindowModal(winSetBatchTerms);
}
end;

tree trSHKObj;
 table SHKObj;
  Fields
   SHKObj.code        'Код'             : [3] , NoProtect, nopickbutton, {Font={BackColor=if(SHKObj.code='',ColorNeed,0)}};
   SHKObj.Name        'Наименование'    : [10], NoProtect, nopickbutton, {Font={BackColor=if(SHKObj.Name='',ColorNeed,0)}};
   if(SHKObj.isGroup = False, tTypeObjWin.Name,'Группа шаблонов')     'Тип документа' : [10], Protect, nopickbutton, {Font={BackColor=if(SHKObj.Name='',ColorNeed,0)}};
   SHKObj.Description 'Описание'        : [15], NoProtect, NoPickButton,NoDel;
   if(SHKObj.isGroup = False, SHKObj.Action,'')       'Действие'       : [7], Protect, NoPickButton,NoDel;
   if(SHKObj.isGroup = False, if (SHKObj.WISACTIVE = 0,'НЕТ','ДА'),'')    'Отображать на ТСД' : [5], Protect, NoPickButton,NoDel;
   if(SHKObj.isGroup = False, if (SHKObj.IsMenuPoint and trim(SHKObj.MenuPointName) <> '' ,'ДА', 'НЕТ'),'')    'Меню на ТСД' : [5], Protect, NoPickButton,NoDel;
end;

tableevent table SHKObj;
  cmTreeTop:
    _cParent := 0;

  cmTreeUp:
    _cParent := SHKObj.cGroup;

  cmTreeDown:
    _cParent := SHKObj.Nrec;

  cmTreeNodeType:
    if SHKObj.isGroup = False then TreeSetNodeType(trSHKObj, ntfText);
end;




Window wnSHK_TMPLT_Edit_group 'Редактирование группы ШК' ;
//Show at (3,5,120,28);
//---------------------------------------------
Screen ScrSHK_Edit_group (,,Sci1Esc);
Table SHKObj;
Fields
 SHKObj_UP.name      : Protect, PickButton;
 SHKObj.code         : NoProtect, #colorneed(SHKObj.code='');
 SHKObj.Name         : NoProtect, #colorneed(SHKObj.Name='');
 SHKObj.Description  : NoProtect, NoPickButton, NoDel;
<<
 `Вышестоящая группа` .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

 `Код` .@@@@@@@ `Наименование`.@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

 `Описание` .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
>>
end;//Screen ScrSHK_getParameter
end;
