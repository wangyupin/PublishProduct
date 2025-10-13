/*eslint-disable */
import { Link } from 'react-router-dom'
import { UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap'
import { ChevronDown, MoreVertical, FileText, Archive, Trash2, ShoppingBag, Copy } from 'react-feather'
import Avatar from '@components/avatar'
import { store } from '@store/store'
import axios from 'axios'
import { agGrid_valueFormatterFunction, renderGoodName } from '@CityAppHelper'

// ** Renders Client Columns
const renderClient = row => {
  const stateNum = Math.floor(Math.random() * 6),
    states = ['light-success', 'light-danger', 'light-warning', 'light-info', 'light-primary', 'light-secondary'],
    color = states[stateNum]
  if (row.avatar?.length) {
    return <Avatar className='me-1 overflow-hidden rounded' img={row.avatar} width='32' height='32' imgClassName='rounded' />
  } else {
    return <Avatar color={color || 'primary'} className='me-1 overflow-hidden rounded' content={row.goodName} initials />
  }
}

const Columns = ({ access, t, setCurrentPage, navigate }) => ([
  {
    headerName: t('goodsInfo.goodID', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'goodID',
    flex: 250,
    minWidth: 250,
    cellRenderer: ({ data }) => (
      <div className='d-flex justify-content-left align-items-center'>
        {renderGoodName(data)}
        <div className='d-flex'>
          <Link
            to={`../MainTableMgmt/GoodsInfo/Single/${data.goodID}`}
            className='text-body'
          >
            <span className='text-primary'>{data.goodID}</span>
          </Link>
        </div>
      </div>
    ),
    headerCheckboxSelection: true,
    checkboxSelection: true,
    showDisabledCheckboxes: true
  },
  {
    headerName: t('goodsInfo.goodName', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'goodName',
    flex: 250,
  },
  {
    headerName: t('goodsInfo.factoryName', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'factoryName',
    flex: 250,
  },
  {
    headerName: t('goodsInfo.brand', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'brandName',
    flex: 150,
  },
  {
    headerName: t('goodsInfo.advicePrice', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'advicePrice',
    flex: 150,
    valueFormatter: agGrid_valueFormatterFunction,
    headerClass: 'ag-right-aligned-header',
    cellClass: 'ag-right-aligned-cell'
  },
  {
    headerName: t('goodsInfo.currency', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'currency',
    flex: 150,
    cellClass: 'ag-right-aligned-cell'
  },
  {
    headerName: t('goodsInfo.intoPrice', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'intoPrice',
    flex: 150,
    valueFormatter: agGrid_valueFormatterFunction,
    headerClass: 'ag-right-aligned-header',
    cellClass: 'ag-right-aligned-cell'
  },
  {
    headerName: t('goodsInfo.cost', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'cost',
    flex: 150,
    valueFormatter: agGrid_valueFormatterFunction,
    headerClass: 'ag-right-aligned-header',
    cellClass: 'ag-right-aligned-cell'
  },
  {
    headerName: t('goodsInfo.s_Ply', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 's_Ply',
    flex: 150,
    valueFormatter: agGrid_valueFormatterFunction,
    headerClass: 'ag-right-aligned-header',
    cellClass: 'ag-right-aligned-cell'
  },
  {
    headerName: t('goodsInfo.price', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'price',
    flex: 150,
    valueFormatter: agGrid_valueFormatterFunction,
    headerClass: 'ag-right-aligned-header',
    cellClass: 'ag-right-aligned-cell'
  },
  {
    headerName: t('goodsInfo.specialPrice', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'specialPrice',
    flex: 150,
    valueFormatter: agGrid_valueFormatterFunction,
    headerClass: 'ag-right-aligned-header',
    cellClass: 'ag-right-aligned-cell'
  },
  {
    headerName: t('goodsInfo.hongLiTimes', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'hongLiTimes',
    flex: 150
  },
  {
    headerName: t('goodsInfo.season', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'season',
    flex: 150
  },
  {
    headerName: t('goodsInfo.material1', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'material1',
    flex: 150
  },
  {
    headerName: t('goodsInfo.p_StyleA', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'p_StyleA',
    flex: 150
  },
  {
    headerName: t('goodsInfo.p_StyleB', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'p_StyleB',
    flex: 150
  },
  {
    headerName: t('goodsInfo.p_StyleC', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'p_StyleC',
    flex: 150
  },
  {
    headerName: t('goodsInfo.p_StyleD', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'p_StyleD',
    flex: 150
  },
  {
    headerName: t('goodsInfo.p_StyleE', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'p_StyleE',
    flex: 150
  },
  {
    headerName: t('goodsInfo.sort01', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'sort01',
    flex: 150
  },
  {
    headerName: t('goodsInfo.sort02', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'sort02',
    flex: 150
  },
  {
    headerName: t('goodsInfo.sort03', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'sort03',
    flex: 150
  },
  {
    headerName: t('goodsInfo.sort04', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'sort04',
    flex: 150
  },
  {
    headerName: t('goodsInfo.sort05', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'sort05',
    flex: 150
  },
  {
    headerName: t('goodsInfo.goodNameUS', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'goodNameUS',
    flex: 150
  },
  {
    headerName: t('goodsInfo.parentID', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'parentID',
    flex: 150
  },
  {
    headerName: t('goodsInfo.currencyRate', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'currencyRate',
    flex: 150
  },
  {
    headerName: t('goodsInfo.tradePrice1', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'tradePrice1',
    flex: 150
  },
  {
    headerName: t('goodsInfo.sizeName', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'sizeName',
    flex: 150
  },
  {
    headerName: t('goodsInfo.yearOfSeason', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'yearOfSeason',
    flex: 150
  },
  {
    headerName: t('goodsInfo.openDate', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'openDate',
    flex: 150
  },
  {
    headerName: t('goodsInfo.sellDateST', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'sellDateST',
    flex: 150
  },
  {
    headerName: t('goodsInfo.remark', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'remark',
    flex: 150
  },
  {
    headerName: t('goodsInfo.goodTags', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'goodTags',
    flex: 150
  },
  {
    headerName: t('goodsInfo.display', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'display',
    flex: 150
  },
  {
    headerName: t('goodsInfo.material', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'material',
    flex: 150
  },
  {
    headerName: t('goodsInfo.isStruct', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'isStruct',
    flex: 150
  },
  {
    headerName: t('goodsInfo.isExclCost', { ns: 'mainTableMgmt' }),
    sortable: true,
    field: 'isExclCost',
    flex: 150
  },
  {
    headerName: '操作',
    flex: 200,
    sortable: false,
    hide: !(access?.ecommerceCreate),
    cellRenderer: ({ data, api }) => (
      <div className='d-flex flex-row align-items-center member-action'>
        {access?.ecommerceCreate && (
          <Link
            href='/'
            className='text-body'
            onClick={async e => {
              e.preventDefault()
              await axios.post(`/api/PublishGoods/GetSubmitMode`, { goodID: data.goodID })
                .then(res => {
                  if (!res.error) {
                    const { mode = 'add', parentID } = res.data.data?.result
                    navigate(`../EcommerceMgmt/PublishGoods/${mode}/${parentID}`, { replace: true })
                  }
                })
            }}
          >
            <ShoppingBag size={17} className='me-50' />
          </Link>
        )}
      </div>
    )
  }
])

export const options = [
  {
    label: '預設',
    disabled: true,
    options: [
      { value: 'goodID', label: '商品型號' },
    ]
  },
  {
    label: '自訂',
    options: [
      { value: 'goodName', label: '品名規格' },
      { value: 'factoryName', label: '供應廠商' },
      { value: 'brandName', label: '品牌' },
      { value: 'advicePrice', label: '建議售價' },
      { value: 'currency', label: '使用幣別' },
      { value: 'intoPrice', label: '起始進價' },
      { value: 'cost', label: '最後進價' },
      { value: 's_Ply', label: '批價折扣' },
      { value: 'price', label: '批價' },
      { value: 'specialPrice', label: '特價' },
      { value: 'hongLiTimes', label: '點數比例' },
      { value: 'season', label: '季別' },
      { value: 'material1', label: '包裝材' },
      { value: 'p_StyleA', label: '材質1' },
      { value: 'p_StyleB', label: '材質2' },
      { value: 'p_StyleC', label: '材質3' },
      { value: 'p_StyleD', label: '屬性' },
      { value: 'p_StyleE', label: '產地' },
      { value: 'sort01', label: '類別1(大類)' },
      { value: 'sort02', label: '類別2(中類)' },
      { value: 'sort03', label: '類別3(小類)' },
      { value: 'sort04', label: '類別4(系列)' },
      { value: 'sort05', label: '類別5(顏色)' },
      { value: 'goodNameUS', label: '品名規格(英)' },
      { value: 'parentID', label: '款式編號' },
      { value: 'currencyRate', label: '使用匯率' },
      { value: 'tradePrice1', label: '幣別進價' },
      { value: 'sizeName', label: '尺寸名稱' },
      { value: 'yearOfSeason', label: '年度' },
      { value: 'openDate', label: '建檔日' },
      { value: 'sellDateST', label: '販售日期' },
      { value: 'remark', label: '備註' },
      { value: 'goodTags', label: '標籤' },
      { value: 'display', label: '型號作廢' },
      { value: 'material', label: '銷貨不轉預調' },
      { value: 'isStruct', label: '組合商品' },
      { value: 'isExclCost', label: '不列入成本計算' }
    ]
  }
]

export const columnFilter = [
  { label: '商品型號', value: 'goodID' },
  { label: '品名規格', value: 'goodName' },
  { label: '供應廠商', value: 'factoryName' },
  { label: '品牌', value: 'brandName' },
  { label: '建議售價', value: 'advicePrice', type: 'number' },
  { label: '歸屬編號', value: 'parentID' },
  { label: '起始進價', value: 'intoPrice', type: 'number' },
  { label: '最後進價', value: 'cost', type: 'number' },
  { label: '批價折扣', value: 's_Ply', type: 'number' },
  { label: '批價', value: 'price', type: 'number' },
  { label: '特價', value: 'specialPrice', type: 'number' },
  { label: '點數比例', value: 'hongLiTimes' },
  { label: '季別', value: 'season' },
  { label: '包裝材', value: 'material1' },
  { label: '材質1', value: 'p_StyleA' },
  { label: '材質2', value: 'p_StyleB' },
  { label: '材質3', value: 'p_StyleC' },
  { label: '屬性', value: 'p_StyleD' },
  { label: '產地', value: 'p_StyleE' },
  { label: '類別1(大類)', value: 'sort01' },
  { label: '類別2(中類)', value: 'sort02' },
  { label: '類別3(小類)', value: 'sort03' },
  { label: '類別4(系列)', value: 'sort04' },
  { label: '類別5(顏色)', value: 'sort05' },
  { label: '條碼號', value: 'bar' }
]

export default Columns
