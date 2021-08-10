using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Microsoft.Win32;

namespace Xamasoft.JsonClassGenerator.WinForms
{
    public partial class MainForm : Form
    {
        private bool preventReentrancy = false;
        public string NameFile
        {
            get; set;
        }

        public string Teste { get; set; }

        public MainForm()
        {
            // `IconTitleFont` is what WinForms *should* be using by default.
            // Need to set `this.Font` first, before `this.InitializeComponent();` to ensure font inheritance by controls in the form.
            this.Font = SystemFonts.IconTitleFont;

            this.InitializeComponent();

            this.ResetFonts();

            // Also: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/how-to-respond-to-font-scheme-changes-in-a-windows-forms-application?view=netframeworkdesktop-4.8
            SystemEvents.UserPreferenceChanged += this.SystemEvents_UserPreferenceChanged;

            //

            this.openButton.Click += this.OpenButton_Click;

            this.optAttribJP    .CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.optAttribJpn   .CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.optAttribNone  .CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.sDKToolStripMenuItem.CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.mSAToolStripMenuItem.CheckedChanged += this.OnAttributesModeCheckedChanged;

            this.optMemberFields.CheckedChanged += this.OnMemberModeCheckedChanged;
            this.optMemberProps .CheckedChanged += this.OnMemberModeCheckedChanged;

            this.optImmutable   .CheckedChanged += this.OnOptionsChanged;
            this.optsPascalCase .CheckedChanged += this.OnOptionsChanged;

            this.wrapText.CheckedChanged += this.WrapText_CheckedChanged;

            this.copyOutput.Click += this.CopyOutput_Click;
            this.copyOutput.Enabled = false;

            this.jsonInputTextbox.TextChanged += this.JsonInputTextbox_TextChanged;
            this.jsonInputTextbox.DragDrop += this.JsonInputTextbox_DragDrop;
            this.jsonInputTextbox.DragOver += this.JsonInputTextbox_DragOver;
            this.textBox1.TextChanged += this.textBox1_TextChanged;
            //this.jsonInputTextbox.paste // annoyingly, it isn't (easily) feasible to hook/detect TextBox paste events, even globally... grrr.

            // Invoke event-handlers to set initial toolstrip text:
            this.optsAttributeMode.Tag = this.optsAttributeMode.Text + ": {0}";
            this.optMembersMode   .Tag = this.optMembersMode   .Text + ": {0}";

            this.OnAttributesModeCheckedChanged( this.optAttribJP   , EventArgs.Empty );
            this.OnMemberModeCheckedChanged    ( this.optMemberProps, EventArgs.Empty );
        }

        private void WrapText_CheckedChanged(Object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;

            // For some reason, toggling WordWrap causes a text selection in `jsonInputTextbox`. So, doing this:
            try
            {
                this.jsonInputTextbox.HideSelection = true;
                // ayayayay: https://stackoverflow.com/questions/1140250/how-to-remove-the-focus-from-a-textbox-in-winforms
                this.ActiveControl = this.toolStrip;

#if WINFORMS_TEXTBOX_GET_SCROLL_POSITION_WORKS_ARGH // It's non-trivial: https://stackoverflow.com/questions/4494162/change-scrollbar-position-in-textbox
                //int idx1 = this.jsonInputTextbox.GetFirstCharIndexOfCurrentLine(); // but what is the "current line"?
                int firstLineCharIndex = -1;
                if( this.jsonInputTextbox.Height > 10 )
                {
                    // https://stackoverflow.com/questions/10175400/maintain-textbox-scroll-position-while-adding-line
                    this.jsonInputTextbox.GetCharIndexFromPosition( new Point( 3, 3 ) );
                }
#endif

                this.jsonInputTextbox   .WordWrap = tsb.Checked;
                this.csharpOutputTextbox.WordWrap = tsb.Checked;

#if WINFORMS_TEXTBOX_GET_SCROLL_POSITION_WORKS_ARGH
                if( firstLineCharIndex > 0 ) // Greater than zero, not -1, because `GetCharIndexFromPosition` returns a meaningless zero sometimes.
                {
                    this.jsonInputTextbox.SelectionStart = firstLineCharIndex;
                    this.jsonInputTextbox.ScrollToCaret();
                }
#endif
            }
            finally
            {
                this.jsonInputTextbox.HideSelection = false;
            }
        }

        #region WinForms Taxes

        private static Font GetMonospaceFont( Single emFontSizePoints )
        {
            // See if Consolas or Lucida Sans Typewriter is available before falling-back:
            String[] preferredFonts = new[] { "Consolas", "Lucida Sans Typewriter" };
            foreach( String fontName in preferredFonts )
            {
                if( TestFont( fontName, emFontSizePoints ) )
                {
                    return new Font( fontName, emFontSizePoints, FontStyle.Regular );
                }
            }

            // Fallback:
            return new Font( FontFamily.GenericMonospace, emSize: emFontSizePoints );
        }

        private static Boolean TestFont( String fontName, Single emFontSizePoints )
        {
            try
            {
                using( Font test = new Font( fontName, emFontSizePoints, FontStyle.Regular ) )
                {
                    return test.Name == fontName;
                }
            }
            catch
            {
                return false;
            }
        }

        private void SystemEvents_UserPreferenceChanged(Object sender, UserPreferenceChangedEventArgs e)
        {
            switch( e.Category )
            {
            case UserPreferenceCategory.Accessibility:
            case UserPreferenceCategory.Window:
            case UserPreferenceCategory.VisualStyle:
            case UserPreferenceCategory.Menu:
                this.ResetFonts();
                break;
            }
        }

        private void ResetFonts()
        {
            this.Font = SystemFonts.IconTitleFont;

            Font monospaceFont = GetMonospaceFont( emFontSizePoints: SystemFonts.IconTitleFont.SizeInPoints );
            this.jsonInputTextbox   .Font = monospaceFont;
            this.csharpOutputTextbox.Font = monospaceFont;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            if( !e.Cancel )
            {
                SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            }
        }

        #endregion

        #region Highlander: There can only be one!

        private void OnAttributesModeCheckedChanged(Object sender, EventArgs e)
        {
            this.WhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohh( (ToolStripMenuItem)sender, defaultItem: this.optAttribJP, parent: this.optsAttributeMode );

            this.GenerateCSharp();
        }

        private void OnMemberModeCheckedChanged(Object sender, EventArgs e)
        {
            this.WhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohh( (ToolStripMenuItem)sender, defaultItem: this.optMemberProps, parent: this.optMembersMode );

            this.GenerateCSharp();
        }

        /// <summary>https://www.youtube.com/watch?v=Qy1J_i32wTg</summary>
        private void WhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohh(ToolStripMenuItem subject, ToolStripMenuItem defaultItem, ToolStripDropDownButton parent)
        {
            if( this.preventReentrancy ) return;
            try
            {
                this.preventReentrancy = true;

                ToolStripMenuItem singleCheckedItem;
                if( subject.Checked )
                {
                    singleCheckedItem = subject;
                    this.UncheckOthers( subject, parent );
                }
                else
                {
                    this.EnsureAtLeast1IsCheckedAfterItemWasUnchecked( subject, defaultItem, parent );
                    singleCheckedItem = parent.DropDownItems.Cast<ToolStripMenuItem>().Single( item => item.Checked );
                }

                String parentTextFormat = (String)parent.Tag;
                parent.Text = String.Format( format: parentTextFormat, arg0: singleCheckedItem.Text );
            }
            finally
            {
                this.preventReentrancy = false;
            }
        }

        private void UncheckOthers( ToolStripMenuItem sender, ToolStripDropDownButton parent )
        {
            foreach( ToolStripMenuItem menuItem in parent.DropDownItems.Cast<ToolStripMenuItem>() ) // I really hate old-style IEnumerable, *grumble*
            {
                if( !Object.ReferenceEquals( menuItem, sender ) )
                {
                    menuItem.Checked = false;
                }
            }
        }

        private void EnsureAtLeast1IsCheckedAfterItemWasUnchecked( ToolStripMenuItem subject, ToolStripMenuItem defaultItem, ToolStripDropDownButton parent )
        {
            int countChecked = parent.DropDownItems.Cast<ToolStripMenuItem>().Count( item => item.Checked );

            if( countChecked == 1 )
            {
                // Is exactly 1 checked already? If so, then NOOP.
            }
            else if( countChecked > 1 )
            {
                // If more than 1 are checked, then check only the default:
                defaultItem.Checked = true;
                this.UncheckOthers( sender: defaultItem, parent );
            }
            else
            {
                // If none are checked, then *if* the unchecked item is NOT the default item, then check the default item:
                if( !Object.ReferenceEquals( subject, defaultItem ) )
                {
                    defaultItem.Checked = true;
                }
                else
                {
                    // Otherwise, check the first non-default item:
                    ToolStripMenuItem nextBestItem = parent.DropDownItems.Cast<ToolStripMenuItem>().First( item => item != defaultItem );
                    nextBestItem.Checked = true;
                }
            }
        }

        #endregion

        private void OnOptionsChanged(Object sender, EventArgs e)
        {
            this.GenerateCSharp();
        }

        #region Drag and Drop

        private void JsonInputTextbox_DragOver(Object sender, DragEventArgs e)
        {
            bool acceptable = 
                e.Data.GetDataPresent( DataFormats.FileDrop ) ||
//              e.Data.GetDataPresent( DataFormats.Text ) ||
//              e.Data.GetDataPresent( DataFormats.OemText ) ||
                e.Data.GetDataPresent( DataFormats.UnicodeText, autoConvert: true )// ||
//              e.Data.GetDataPresent( DataFormats.Html ) ||
//              e.Data.GetDataPresent( DataFormats.StringFormat ) ||
//              e.Data.GetDataPresent( DataFormats.Rtf )
            ;

            if( acceptable )
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void JsonInputTextbox_DragDrop(Object sender, DragEventArgs e)
        {
            if( e.Data.GetDataPresent( DataFormats.FileDrop ) )
            {
                String[] fileNames = (String[])e.Data.GetData( DataFormats.FileDrop );
                if( fileNames.Length >= 1 )
                {
                    // hmm, maybe allow multiple files by concatenating them all into a horrible JSON array? :D
                    this.TryLoadJsonFile( fileNames[0] );
                }
            }
            else if( e.Data.GetDataPresent( DataFormats.UnicodeText, autoConvert: true ) )
            {
                this.statusStrip.Text = "";

                String text = (String)e.Data.GetData( DataFormats.UnicodeText, autoConvert: true );
                if( text != null )
                {
                    this.jsonInputTextbox.Text = text; // This will invoke `GenerateCSharp()`.

                    this.statusStrip.Text = "Loaded JSON from drag and drop data.";
                }
            }
        }

        /// <summary>This regex won't match <c>\r\n</c>, only <c>\n</c>.</summary>
        private static readonly Regex _onlyUnixLineBreaks = new Regex( "(?<!\r)\n", RegexOptions.Compiled ); // Don't use `[^\r]?\n` because it *will* match `\r\n`, and don't use `[^\r]\n` because it won't match a leading `$\n` in a file.

        private static String RepairLineBreaks( String text )
        {
            if( _onlyUnixLineBreaks.IsMatch( text ) )
            {
                return _onlyUnixLineBreaks.Replace( text, replacement: "\r\n" );
            }

            return text;
        }

        #endregion

        #region Open JSON file

        private void OpenButton_Click(Object sender, EventArgs e)
        {
            //if( this.ofd.ShowDialog( owner: this ) == DialogResult.OK )
            //{
            //    this.TryLoadJsonFile( this.ofd.FileName );
            //}

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Custom Description";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.Teste = fbd.SelectedPath;
                this.textBox1.ReadOnly = false;
                this.jsonInputTextbox.ReadOnly = false;
            }

            this.csharpOutputTextbox.Text = this.Teste;
        }

        private void TryLoadJsonFile( String filePath )
        {
            if ( String.IsNullOrWhiteSpace( filePath ) )
            {
                this.csharpOutputTextbox.Text = "Error: an empty file path was specified.";
            }
//          else if ( filePath.IndexOfAny( Path.GetInvalidFileNameChars() ) > -1 )
//          {
//              const String fmt = "Invalid file path: \"{0}\"";
//              this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, fmt, filePath );
//          }
            else
            {
                FileInfo jsonFileInfo;
                try
                {
                    jsonFileInfo = new FileInfo( filePath );
                }
                catch( Exception ex )
                {
                    const String fmt = "Invalid file path: \"{0}\"\r\n{1}";
                    this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, fmt, filePath, ex.ToString() );
                    return;
                }

                this.TryLoadJsonFile( jsonFileInfo );
            }
        }

        private void TryLoadJsonFile( FileInfo jsonFile )
        {
            if( jsonFile is null ) return;

            this.statusStrip.Text = "";

            try
            {
                jsonFile.Refresh();
                if( jsonFile.Exists )
                {
                    String jsonText = File.ReadAllText( jsonFile.FullName );
                    this.jsonInputTextbox.Text = jsonText; // This will invoke `GenerateCSharp()`.

                    this.statusStrip.Text = "Loaded \"" + jsonFile.FullName + "\" successfully.";
                }
                else
                {
                    this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, "Error: File \"{0}\" does not exist.", jsonFile.FullName );
                }
            }
            catch( Exception ex )
            {
                const String fmt = "Error loading file: \"{0}\"\r\n{1}";

                this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, fmt, jsonFile.FullName, ex.ToString() );
            }
        }

        #endregion

        private void ConfigureGenerator( IJsonClassGeneratorConfig config )
        {
            config.UsePascalCase = this.optsPascalCase.Checked;
            config.InputFileName = NameFile;
            //
            if (this.sDKToolStripMenuItem.Checked)
            {
                config.UseJsonAttributes = false;
                config.UseJsonPropertyName = false;
                config.UseJsonPropertyNamesForSDK = false;
                config.UseJsonPropertyNamesForMSA = true;

            }
            if (this.sDKToolStripMenuItem.Checked)
            {
                config.UseJsonAttributes = false;
                config.UseJsonPropertyName = false;
                config.UseJsonPropertyNamesForSDK = true;
                config.UseJsonPropertyNamesForMSA = false;

            }
            else if( this.optAttribJP.Checked )
            {
                config.UseJsonAttributes   = true;
                config.UseJsonPropertyName = false;
                config.UseJsonPropertyNamesForSDK = false;
                config.UseJsonPropertyNamesForMSA = false;
            }
            else if( this.optAttribJpn.Checked )
            {
                config.UseJsonAttributes   = false;
                config.UseJsonPropertyName = true;
                config.UseJsonPropertyNamesForSDK = false;
                config.UseJsonPropertyNamesForMSA = false;
            }
            else// implicit: ( this.optAttribNone.Checked )
            {
                config.UseJsonAttributes   = false;
                config.UseJsonPropertyName = false;
                config.UseJsonPropertyNamesForSDK = false;
                config.UseJsonPropertyNamesForMSA = false;
            }

            //

            if( this.optMemberProps.Checked )
            {
                config.UseProperties = true;
                config.UseFields     = false;
            }
            else// implicit: ( this.optMemberFields.Checked )
            {
                config.UseProperties = false;
                config.UseFields     = true;
            }

            config.ImmutableClasses = this.optImmutable.Checked;
        }

        private void JsonInputTextbox_TextChanged(Object sender, EventArgs e)
        {
            if( this.preventReentrancy ) return;
            this.preventReentrancy = true;
            try
            {
                this.jsonInputTextbox.Text = RepairLineBreaks( this.jsonInputTextbox.Text );

                this.GenerateCSharp();
            }
            finally
            {
                this.preventReentrancy = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            NameFile = this.textBox1.Text;
        }

        private async void GenerateCSharp()
        {
            this.copyOutput.Enabled = false;

            String jsonText = this.jsonInputTextbox.Text;
            if( String.IsNullOrWhiteSpace( jsonText ) )
            {
                this.csharpOutputTextbox.Text = String.Empty;
                return;
            }

            JsonClassGenerator generator = new JsonClassGenerator();
            this.ConfigureGenerator( generator );

            try
            {
                var errorMessage = string.Empty;
                StringBuilder sb = new StringBuilder();

                if (generator.UseJsonPropertyNamesForSDK)
                {
                    var args = textBox1.Text.Split('|');

                    generator.isInput = args[0].Equals("Input");
                    generator.typeFile = args[0];
                    generator.InputFileProductName = args[1];
                    generator.InputFileAreaName = args[2];
                    generator.InputFileName = args[3];
                    generator.InputType = args[4];

                    sb = generator.GenerateClasses( jsonText, errorMessage: out errorMessage );
                   
                    Directory.SetCurrentDirectory(this.Teste);

                    var repoFolder = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                    string finalFolder = Path.Combine(repoFolder, "ParticularesServices");

                    Directory.CreateDirectory(finalFolder);

                    generator.UseJsonPropertyNamesForSDK = false;
                    generator.UseJsonAttributes = false;
                    generator.UseJsonPropertyName = false;

                    if (generator.isInput)
                    {
                        string inputDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Business\Models\Input");
                        Directory.CreateDirectory(inputDirectory);
                        var businessInput = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseBusinessInput.pp"));
                        businessInput = businessInput.Replace("$Class$", sb.ToString());
                        businessInput = businessInput.Replace("$Product$", generator.InputFileProductName);
                        File.WriteAllText(Path.Combine(inputDirectory, $"{generator.InputFileName}{args[0]}.cs"), businessInput);

                        string hmbIntegrationTestsDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Test\HomeBanking\Integration");
                        Directory.CreateDirectory(hmbIntegrationTestsDirectory);

                        string hmbUnitTestsDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Test\HomeBanking\Unit");
                        Directory.CreateDirectory(hmbUnitTestsDirectory);

                        string mobIntegrationTestsDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Test\CAMobile\Integration");
                        Directory.CreateDirectory(mobIntegrationTestsDirectory);

                        string mobUnitTestsDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Test\CAMobile\Unit");
                        Directory.CreateDirectory(mobUnitTestsDirectory);

                        string requestDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Models\Request");
                        Directory.CreateDirectory(requestDirectory);

                        string serviceDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Business\Services\Implementations");
                        Directory.CreateDirectory(serviceDirectory);

                        var requestFilename = $"{generator.InputFileName}Request";
                        var modelRequest = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseModelRequest.pp"));
                        sb = generator.GenerateClasses(jsonText, errorMessage: out string errorMessage1, name:requestFilename );
                        modelRequest = modelRequest.Replace("$Class$", sb.ToString());
                        modelRequest = modelRequest.Replace("$Product$", generator.InputFileProductName);
                        File.WriteAllText(Path.Combine(requestDirectory, $"{requestFilename}.cs"), modelRequest);

                        var serviceSource = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseServiceSource.pp"));
                        var hmbIntegrationTest = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseHmbIntegrationTest.pp"));
                        var hmbUnitTest = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseHmbUnitTest.pp"));
                        var mobIntegrationTest = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseMobileIntegrationTest.pp"));
                        var mobUnitTest = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseMobileUnitTest.pp"));

                        var serviceFilename = $"{generator.InputFileName}Service";
                        var testFileName = $"{generator.InputFileName}Test";

                        serviceSource = serviceSource.Replace("$Product$", generator.InputFileProductName);
                        serviceSource = serviceSource.Replace("$Area$", generator.InputFileAreaName);
                        serviceSource = serviceSource.Replace("$ClassName$", generator.InputFileName);
                        serviceSource = serviceSource.Replace("$Type$", generator.InputType);
                        File.WriteAllText(Path.Combine(serviceDirectory, $"{serviceFilename}.cs"), serviceSource);

                        hmbIntegrationTest = hmbIntegrationTest.Replace("$Product$", generator.InputFileProductName);
                        hmbIntegrationTest = hmbIntegrationTest.Replace("$Area$", generator.InputFileAreaName);
                        hmbIntegrationTest = hmbIntegrationTest.Replace("$ClassName$", generator.InputFileName);
                        File.WriteAllText(Path.Combine(hmbIntegrationTestsDirectory, $"{testFileName}.cs"), hmbIntegrationTest);

                        hmbUnitTest = hmbUnitTest.Replace("$Product$", generator.InputFileProductName);
                        hmbUnitTest = hmbUnitTest.Replace("$Area$", generator.InputFileAreaName);
                        hmbUnitTest = hmbUnitTest.Replace("$ClassName$", generator.InputFileName);
                        File.WriteAllText(Path.Combine(hmbUnitTestsDirectory, $"{testFileName}.cs"), hmbUnitTest);

                        mobIntegrationTest = mobIntegrationTest.Replace("$Product$", generator.InputFileProductName);
                        mobIntegrationTest = mobIntegrationTest.Replace("$Area$", generator.InputFileAreaName);
                        mobIntegrationTest = mobIntegrationTest.Replace("$ClassName$", generator.InputFileName);
                        File.WriteAllText(Path.Combine(mobIntegrationTestsDirectory, $"{testFileName}.cs"), mobIntegrationTest);

                        mobUnitTest = mobUnitTest.Replace("$Product$", generator.InputFileProductName);
                        mobUnitTest = mobUnitTest.Replace("$Area$", generator.InputFileAreaName);
                        mobUnitTest = mobUnitTest.Replace("$ClassName$", generator.InputFileName);
                        File.WriteAllText(Path.Combine(mobUnitTestsDirectory, $"{testFileName}.cs"), mobUnitTest);

                        string areaServiceDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Business\Services");
                        var areaServiceSource = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\areaService.pp"));
                        areaServiceSource = areaServiceSource.Replace("$Product$", generator.InputFileProductName);
                        areaServiceSource = areaServiceSource.Replace("$Area$", generator.InputFileAreaName);
                        areaServiceSource = areaServiceSource.Replace("$ClassName$", generator.InputFileName);
                        
                        var areaServiceImplementationSource = File.ReadAllText(Path.Combine(repoFolder, $@"ServiceGenerator\serviceImplementation.pp"));
                        areaServiceImplementationSource = areaServiceImplementationSource.Replace("//$Replace$", areaServiceSource);
                        File.WriteAllText(Path.Combine(areaServiceDirectory, $"{generator.InputFileAreaName}Service.cs"), areaServiceImplementationSource);

                    }
                    else
                    {
                        string outputDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Business\Models\Output");
                        Directory.CreateDirectory(outputDirectory);
                        var businessOutput = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseModelResult.pp"));
                        businessOutput = businessOutput.Replace("$Class$", sb.ToString());
                        businessOutput = businessOutput.Replace("$Product$", generator.InputFileProductName);
                        File.WriteAllText(Path.Combine(outputDirectory, $"{generator.InputFileName}{args[0]}.cs"), businessOutput);

                        string resultDirectory = Path.Combine(finalFolder, $@"{generator.InputFileProductName}.Models\Result");
                        Directory.CreateDirectory(resultDirectory);

                        var resultFilename = $"{generator.InputFileName}Result"; 
                        sb = generator.GenerateClasses(jsonText, errorMessage: out string errorMessage1, name:resultFilename);
                        var modelResult = File.ReadAllText(Path.Combine(repoFolder, @"ServiceGenerator\baseModelResult.pp"));
                        modelResult = modelResult.Replace("$Class$", sb.ToString());
                        modelResult = modelResult.Replace("$Product$", generator.InputFileProductName);
                        File.WriteAllText(Path.Combine(resultDirectory, $"{resultFilename}.cs"), modelResult);
                    }                                                                             
                }
                if (generator.UseJsonPropertyNamesForMSA)
                {
                    var args = textBox1.Text.Split('|');

                    generator.isInput = args[0].Equals("Input");
                    generator.InputFileProductName = args[1];
                    generator.InputFileAreaName = args[2];
                    generator.InputFileName = args[3];
                    sb = generator.GenerateClasses( jsonText, errorMessage: out errorMessage );


                }

                if ( !String.IsNullOrWhiteSpace( errorMessage ) )
                {
                    this.csharpOutputTextbox.Text = "Error:\r\n" + errorMessage;
                }
                else
                {
                    this.csharpOutputTextbox.Text = sb.ToString();
                    this.copyOutput.Enabled = true;
                }
            }
            catch( Exception ex )
            {
                this.csharpOutputTextbox.Text = "Error:\r\n" + ex.ToString();
            }
        }

        private void CopyOutput_Click(Object sender, EventArgs e)
        {
            if( this.csharpOutputTextbox.Text?.Length > 0 )
            {
                Clipboard.SetText( this.csharpOutputTextbox.Text, TextDataFormat.UnicodeText );
            }
        }

        private void mSAToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
