# 本機安裝步驟

## 新增服務
```
	sc create LocalPortService binpath= "C:\Users\yourPlace\LocalPortService.exe" type= own start= auto
```
## 刪除服務
```
	sc delete LocalPortService
```

## 刪除服務後新增失敗
	* 關閉'服務'視窗後再重新啟動'服務'視窗
		* 再次執行一次上面指令
## 啟動服務
	*找到服務LocalPortService右鍵點選'啟動'
