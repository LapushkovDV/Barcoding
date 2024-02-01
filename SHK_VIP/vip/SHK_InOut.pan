#declare colorneed(FldCondition)
{Font={BackColor=if(#FldCondition,ColorNeed,0)}}
#end
Window wnSHK_getParameter 'Редактирование параметров объекта ШК' ;
//Show at (3,5,120,28);
//---------------------------------------------
Screen SHK_getParameter (,,Sci178Esc);
//Show at (,,,2);
Table SHKObj;
Fields
 SHKObj.code        : skip, #colorneed(SHKObj.code='');
 SHKObj.Name        : skip, #colorneed(SHKObj.Name='');
 tTypeObjWin.name   : skip, #colorneed(not isvalidall(tntTypeObjWin));
 SHKObj.SYSNAMETBL+'.'+SHKObj.SYSNAME  : skip, #colorneed(SHKObj.SYSNAMETBL = '' or SHKObj.SYSNAME = '');
 SHKObj.Action ('Уникальный идентификатор (по нему идет сопоставление, что именно запрашивается)'): Skip, #colorneed(SHKObj.Action='');
 _Ident            : Noprotect, #colorneed(_Ident = '');
  co_BatchMode     : skip;
 _FileIN : Protect, Pickbutton;
buttons
  cmValue1,,,'Сформировать JSON OBJECTDESCRIPTION',,;
  cmValue2,,,'Сформировать JSON OBJECTVALUES',,;
  cmValue3,,,'Загрузить JSON',,;
<<
 `Код` .@@@@@@@ `Наименование`.@@@@@@@@@@@@@@@@@@@@@@@    Тип документа .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
  Поле идентификации объекта .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 `Идентификатор OBJECTACTION`.@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

     `введите идентификатор` .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ (для пакетной выгрузки впишите .@@@@@@@@@@@)

    <.Сформировать JSON OBJECTDESCRIPTION.>      <.Сформировать JSON OBJECTVALUES.>

Проверка загрузки измененного файла JSON:
    файл .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    <.Загрузить.>

>>
end;//Screen ScrRaiseEdit
end;
windowevent wnSHK_getParameter ;
cmValue1: {
 var _JSONfile : string = GenerateJSON_DescriptionFILE(SHKObj.Action);

 processtext(_JSONfile, vfNewTitle Or vfMacroSize, 'Сгенерированный JSON OBJECTDESCRIPTION');

}
cmValue2: {


 if wgettune('SHK.CONNECTIONTYPE') = 0 {
    var _JSONfile : string = GenerateJSON_VALUESFILE(SHKObj.Action, _Ident);
    processtext(_JSONfile, vfNewTitle Or vfMacroSize, 'Сгенерированный JSON OBJECTVALUES');
   }
   else {
   var _isPathOK   : boolean = checkDestPathfromTune;
    generate_batch_files_group_TSD_ARM_USER(SHKObj.Action);
    if _isPathOK then
      message('Файлы выгружены в папку "' +
       ''#13'' + get_destFolder + '"');
   }
}
cmValue3: {
 if _FileIN = '' then {
   message('Выберите файл',error);
   stop; abort; exit
 }
 var _JSONfile : string = MakeAction_FromJSONFILE('TEST','TEST',_FileIN);
 processtext(_JSONfile, vfNewTitle Or vfMacroSize, 'Ответ на загрузку JSON');
}
end;
