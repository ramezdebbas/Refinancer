using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Executions",
                 "Executions",
                 "Assets/10.jpg",
                 "Refinancing may refer to the replacement of an existing debt obligation with a debt obligation under different terms. The terms and conditions of refinancing may vary widely by country, province, or state, based on several economic factors.");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Introduction",
                 "Introduction",
                 "Assets/11.jpg",
                 "Refinancing may refer to the replacement of an existing debt obligation with a debt obligation under different terms. The terms and conditions of refinancing may vary widely by country, province, or state, based on several economic factors.",
                 "\n\n\n\n\n\n\n\n\nRefinancing may refer to the replacement of an existing debt obligation with a debt obligation under different terms. The terms and conditions of refinancing may vary widely by country, province, or state, based on several economic factors such as, inherent risk, projected risk, political stability of a nation, currency stability, banking regulations, borrower's credit worthiness, and credit rating of a nation. In many industrialized nations, a common form of refinancing is for a place of primary residency mortgage.If the replacement of debt occurs under financial distress, refinancing might be referred to as debt restructuring.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Risks",
                 "Risks",
                 "Assets/12.jpg",
                 "Most fixed-term loans have penalty clauses (call provisions) that are triggered by an early repayment of the loan, in part or in full, as well as closing fees. There will also be transaction fees on the refinancing.",
                 "\n\n\n\n\n\n\n\n\nMost fixed-term loans have penalty clauses (call provisions) that are triggered by an early repayment of the loan, in part or in full, as well as closing fees. There will also be transaction fees on the refinancing. These fees must be calculated before embarking on a loan refinancing, as they can wipe out any savings generated through refinancing. Penalty clauses are only applicable to loans paid off prior to maturity. If a loan is paid off upon maturity it is a new financing, not a refinancing, and all terms of the prior obligation terminate when the new financing funds to pay off the prior debt.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Points",
                 "Points",
                 "Assets/13.jpg",
                 "Refinancing lenders often require a percentage of the total loan amount as an upfront payment. Typically, this amount is expressed in points (or premiums). 1 point = 1% of the total loan amount.",
                 "\n\n\n\n\n\n\n\n\nRefinancing lenders often require a percentage of the total loan amount as an upfront payment. Typically, this amount is expressed in points (or premiums). 1 point = 1% of the total loan amount. More points (i.e. a larger upfront payment) will usually result in a lower interest rate. Some lenders will offer to finance parts of the loan themselves, thus generating so-called negative points (i.e. discounts).",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "No Closing Cost",
                 "No Closing Cost",
                 "Assets/14.jpg",
                 "Borrowers with this type of refinancing typically pay few if any upfront fees to get the new mortgage loan. This type of refinance can be beneficial provided the prevailing market rate is lower than the borrower's existing rate by a formula determined by the lender offering the loan.",
                 "\n\n\n\n\n\n\n\n\nBorrowers with this type of refinancing typically pay few if any upfront fees to get the new mortgage loan. This type of refinance can be beneficial provided the prevailing market rate is lower than the borrower's existing rate by a formula determined by the lender offering the loan. Before you read any further do not provide any lender with a credit card number until they have provided you with a Good Faith Estimate verifying it is truly a 0 cost loan. The appraisal fee cannot be paid for by the lender or broker so this will always show up in the total settlement charges at the bottom of your GFE.\n\nThis can be an excellent choice in a declining market or if you are not sure you will hold the loan long enough to recoup the closing cost before you refinance or pay it off. For example, you plan on selling your home in three years, but it will take five years to recoup the closing cost. This could prevent you from considering a refinance, however if you take the zero closing cost option, you can lower your interest rate without taking any risk of losing money.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Cash out refinancing",
                 "Cash out refinancing",
                 "Assets/15.jpg",
                 "Cash out refinancing (in the case of real property) occurs when a loan is taken out on property already owned, and the loan amount is above and beyond the cost of transaction, payoff of existing liens, and related expenses.",
                 "\n\n\n\n\n\n\n\n\nCash out refinancing (in the case of real property) occurs when a loan is taken out on property already owned, and the loan amount is above and beyond the cost of transaction, payoff of existing liens, and related expenses.\n\nStrictly speaking all refinancing of debt is cash-out, when funds retrieved are utilized for anything other than repaying an existing lien. In the case of common usage of the term, cash out refinancing refers to when equity is liquidated from a property above and beyond sum of the payoff of existing loans held in lien on the property, loan fees, costs associated with the loan, taxes, insurance, tax reserves, insurance reserves, and in the past any other non-lien debt held in the name of the owner being paid by loan proceeds.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Nonrecourse debt",
                 "Nonrecourse debt",
                 "Assets/16.jpg",
                 "Non-recourse debt or a non-recourse loan is a secured loan (debt) that is secured by a pledge of collateral, typically real property, but for which the borrower is not personally liable. If the borrower defaults, the lender/issuer can seize the collateral, but the lender's recovery is limited to the collateral. ",
                 "\n\n\n\n\n\n\n\n\n\n\n\n\nNon-recourse debt or a non-recourse loan is a secured loan (debt) that is secured by a pledge of collateral, typically real property, but for which the borrower is not personally liable. If the borrower defaults, the lender/issuer can seize the collateral, but the lender's recovery is limited to the collateral. Thus, non-recourse debt is typically limited to 50% or 60% loan-to-value ratios,[citation needed] so that the property itself provides overcollateralization of the loan. The incentives for the parties are at an intermediate position between those of a full recourse secured loan and a totally unsecured loan. While the borrower is in first loss position, the lender also assumes significant risk, so the lender must underwrite the loan with much more care than in a full recourse loan. This typically requires that the lender have significant domain expertise and financial modeling expertise.",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Directives",
                 "Directives",
                 "Assets/20.jpg",
                 "Debt restructuring is a process that allows a private or public company – or a sovereign entity – facing cash flow problems and financial distress, to reduce and renegotiate its delinquent debts in order to improve or restore liquidity and rehabilitate so that it can continue its operations.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Debt restructuring",
                 "Debt restructuring",
                 "Assets/21.jpg",
                 "Debt restructuring is a process that allows a private or public company – or a sovereign entity – facing cash flow problems and financial distress, to reduce and renegotiate its delinquent debts in order to improve or restore liquidity and rehabilitate so that it can continue its operations.",
                 "\n\n\n\n\n\n\n\n\nDebt restructuring is a process that allows a private or public company – or a sovereign entity – facing cash flow problems and financial distress, to reduce and renegotiate its delinquent debts in order to improve or restore liquidity and rehabilitate so that it can continue its operations.Replacement of old debt by new debt when not under financial distress is referred to as refinancing. Out-of court restructurings, also known as workouts, are increasingly becoming a global reality.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Mortgage loan",
                 "Mortgage loan",
                 "Assets/22.jpg",
				 "A mortgage loan is a loan secured by real property through the use of a mortgage note which evidences the existence of the loan and the encumbrance of that realty through the granting of a mortgage which secures the loan. However, the word mortgage alone, in everyday usage, is most often used to mean mortgage loan.",
                 "\n\n\n\n\n\n\n\n\nA mortgage loan is a loan secured by real property through the use of a mortgage note which evidences the existence of the loan and the encumbrance of that realty through the granting of a mortgage which secures the loan. However, the word mortgage alone, in everyday usage, is most often used to mean mortgage loan.The word mortgage is a French Law term meaning death contract, meaning that the pledge ends (dies) when either the obligation is fulfilled or the property is taken through foreclosure.\n\nA home buyer or builder can obtain financing (a loan) either to purchase or secure against the property from a financial institution, such as a bank or credit union, either directly or indirectly through intermediaries. Features of mortgage loans such as the size of the loan, maturity of the loan, interest rate, method of paying off the loan, and other characteristics can vary considerably.",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "Refinancing risk",
                 "Refinancing risk",
                 "Assets/23.jpg",
                 "In banking and finance, refinancing risk is the possibility that a borrower cannot refinance by borrowing to repay existing debt. Many types of commercial lending incorporate balloon payments at the point of final maturity; often, the intention or assumption is that the borrower will take out a new loan to pay the existing lenders.",
                 "\n\n\n\n\n\n\n\n\nIn banking and finance, refinancing risk is the possibility that a borrower cannot refinance by borrowing to repay existing debt. Many types of commercial lending incorporate balloon payments at the point of final maturity; often, the intention or assumption is that the borrower will take out a new loan to pay the existing lenders.\n\nA borrower that cannot refinance their existing debt and does not have sufficient funds on hand to pay their lenders may have a liquidity problem. The borrower may be considered technically insolvent: even though their assets are greater than their liabilities, they cannot raise the liquid funds to pay their creditors. Insolvency may lead to bankruptcy, even when the borrower has a positive net worth.\n\nIn order to repay the debt at maturity, the borrower that cannot refinance may be forced into a fire sale of assets at a low price, including the borrower's own home and productive assets such as factories and plants.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Refunding",
                 "Refunding",
                 "Assets/24.jpg",
                 "Refunding occurs when an entity that has issued callable bonds calls those debt securities from the debt holders with the express purpose of reissuing new debt at a lower coupon rate. In essence, the issue of new, lower-interest debt allows the company to prematurely refund the older, higher-interest debt.",
                 "\n\n\n\n\n\n\n\n\nRefunding occurs when an entity that has issued callable bonds calls those debt securities from the debt holders with the express purpose of reissuing new debt at a lower coupon rate. In essence, the issue of new, lower-interest debt allows the company to prematurely refund the older, higher-interest debt. On the contrary, NonRefundable Bonds may be callable but they cannot be re-issued with a lower coupon rate. i.e. They cannot be refunded.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Yield Spread Premium",
                 "Yield Spread Premium",
                 "Assets/25.jpg",
                 "A yield spread premium (YSP) is the money or rebate paid to a mortgage broker for giving a borrower a higher interest rate on a loan in exchange for lower up front costs, generally paid in Origination fees, Broker fees or Discount Points. This “may [be used to] wipe out or offset other loan costs, like Loan Level Pricing Adjustments",
                 "\n\n\n\n\n\n\n\n\nA yield spread premium (YSP) is the money or rebate paid to a mortgage broker for giving a borrower a higher interest rate on a loan in exchange for lower up front costs, generally paid in Origination fees, Broker fees or Discount Points. This “may [be used to] wipe out or offset other loan costs, like Loan Level Pricing Adjustments (instituted by FNMA).\n\nThe YSP is derived through the realization of a market 'price' for a loan that is above 100%. For example, a $300,000 loan with a price when sold of 101.00% would 'yield' a 1% rebate to the originator. It is important to understand that the term 'originator' refers to either a retail bank or mortgage broker. The characteristics of a loan contribute to the price offered, such as the interest rate attached, the credit score of the borrower, purchase money versus a cash-out refinance, or a streamline refinance (which lowers the price because it is typically not accompanied by a property appraisal). Higher credit scores may add 0.25% to the price, while a lower one may cost up to 3.00% - which requires the borrower to either pay a discount fee to cover the loss to the lender when the mortgage is sold, or increasing the interest rate to absorb the risk for the mortgage security investor.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Interest rate",
                 "Interest rate",
                 "Assets/26.jpg",
                 "An interest rate is the rate at which interest is paid by borrowers for the use of money that they borrow from a lender. Specifically, the interest rate (I/m) is a percent of principal (P) paid a certain amount of times (m) per period (usually quoted per annum).",
                 "\n\n\n\n\n\n\n\n\nAn interest rate is the rate at which interest is paid by borrowers for the use of money that they borrow from a lender. Specifically, the interest rate (I/m) is a percent of principal (P) paid a certain amount of times (m) per period (usually quoted per annum). For example, a small company borrows capital from a bank to buy new assets for its business, and in return the lender receives interest at a predetermined interest rate for deferring the use of funds and instead lending it to the borrower. Interest rates are normally expressed as a percentage of the principal for a period of one year.",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
