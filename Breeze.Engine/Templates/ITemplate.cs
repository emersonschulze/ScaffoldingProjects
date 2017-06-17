namespace Breeze.Engine.Templates
{
    public interface ITemplate
    {
        MetaDataDefinition GetMetadata();

        string TransformText();

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        void Write(string textToAppend);

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        void WriteLine(string textToAppend);

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        void Write(string format, params object[] args);

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        void WriteLine(string format, params object[] args);

        /// <summary>
        /// Increase the indent
        /// </summary>
        void PushIndent(string indent);

        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        string PopIndent();

        /// <summary>
        /// Remove any indentation
        /// </summary>
        void ClearIndent();
    }
}