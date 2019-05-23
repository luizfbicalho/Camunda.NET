using System.Text;
using System.Threading;
using System.IO;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.commons.utils
{

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class IoUtil
	{

	  private static readonly IoUtilLogger LOG = UtilsLogger.IO_UTIL_LOGGER;
	  public static readonly Charset ENCODING_CHARSET = Charset.forName("UTF-8");

	  /// <summary>
	  /// Returns the input stream as <seealso cref="string"/>.
	  /// </summary>
	  /// <param name="inputStream"> the input stream </param>
	  /// <returns> the input stream as <seealso cref="string"/>. </returns>
	  public static string inputStreamAsString(Stream inputStream)
	  {
		return StringHelper.NewString(inputStreamAsByteArray(inputStream), ENCODING_CHARSET);
	  }

	  /// <summary>
	  /// Returns the input stream as <seealso cref="sbyte[]"/>.
	  /// </summary>
	  /// <param name="inputStream"> the input stream </param>
	  /// <returns> the input stream as <seealso cref="sbyte[]"/>. </returns>
	  public static sbyte[] inputStreamAsByteArray(Stream inputStream)
	  {
		MemoryStream os = new MemoryStream();
		try
		{
		  sbyte[] buffer = new sbyte[16 * 1024];
		  int read;
		  while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
		  {
			os.Write(buffer, 0, read);
		  }
		  return os.toByteArray();
		}
		catch (IOException e)
		{
		  throw LOG.unableToReadInputStream(e);
		}
		finally
		{
		  closeSilently(inputStream);
		}
	  }

	  /// <summary>
	  /// Returns the <seealso cref="Reader"/> content as <seealso cref="string"/>.
	  /// </summary>
	  /// <param name="reader"> the <seealso cref="Reader"/> </param>
	  /// <returns> the <seealso cref="Reader"/> content as <seealso cref="string"/> </returns>
	  public static string readerAsString(Reader reader)
	  {
		StringBuilder buffer = new StringBuilder();
		char[] chars = new char[16 * 1024];
		int numCharsRead;
		try
		{
		  while ((numCharsRead = reader.read(chars, 0, chars.Length)) != -1)
		  {
			buffer.Append(chars, 0, numCharsRead);
		  }
		  return buffer.ToString();
		}
		catch (IOException e)
		{
		  throw LOG.unableToReadFromReader(e);
		}
		finally
		{
		  closeSilently(reader);
		}
	  }

	  /// <summary>
	  /// Returns the <seealso cref="string"/> as <seealso cref="System.IO.Stream_Input"/>.
	  /// </summary>
	  /// <param name="string"> the <seealso cref="string"/> to convert </param>
	  /// <returns> the <seealso cref="System.IO.Stream_Input"/> containing the <seealso cref="string"/> </returns>
	  public static Stream stringAsInputStream(string @string)
	  {
		return new MemoryStream(@string.GetBytes(ENCODING_CHARSET));
	  }

	  /// <summary>
	  /// Close a closable ignoring any IO exception.
	  /// </summary>
	  /// <param name="closeable"> the closable to close </param>
	  public static void closeSilently(System.IDisposable closeable)
	  {
		try
		{
		  if (closeable != null)
		  {
			closeable.Dispose();
		  }
		}
		catch (IOException)
		{
		  // ignore
		}
	  }

	  /// <summary>
	  /// Returns the content of a file with specified filename
	  /// </summary>
	  /// <param name="filename"> name of the file to load </param>
	  /// <returns> Content of the file as <seealso cref="string"/> </returns>
	  public static string fileAsString(string filename)
	  {
		File classpathFile = getClasspathFile(filename);
		return fileAsString(classpathFile);
	  }

	  /// <summary>
	  /// Returns the content of a <seealso cref="File"/>.
	  /// </summary>
	  /// <param name="file"> the file to load </param>
	  /// <returns> Content of the file as <seealso cref="string"/> </returns>
	  public static string fileAsString(File file)
	  {
		try
		{
		  return inputStreamAsString(new FileStream(file, FileMode.Open, FileAccess.Read));
		}
		catch (FileNotFoundException e)
		{
		  throw LOG.fileNotFoundException(file.AbsolutePath, e);
		}
	  }

	  /// <summary>
	  /// Returns the content of a <seealso cref="File"/>.
	  /// </summary>
	  /// <param name="file"> the file to load </param>
	  /// <returns> Content of the file as <seealso cref="string"/> </returns>
	  public static sbyte[] fileAsByteArray(File file)
	  {
		try
		{
		  return inputStreamAsByteArray(new FileStream(file, FileMode.Open, FileAccess.Read));
		}
		catch (FileNotFoundException e)
		{
		  throw LOG.fileNotFoundException(file.AbsolutePath, e);
		}
	  }


	  /// <summary>
	  /// Returns the input stream of a file with specified filename
	  /// </summary>
	  /// <param name="filename"> the name of a <seealso cref="File"/> to load </param>
	  /// <returns> the file content as input stream </returns>
	  /// <exception cref="IoUtilException"> if the file cannot be loaded </exception>
	  public static Stream fileAsStream(string filename)
	  {
		File classpathFile = getClasspathFile(filename);
		return fileAsStream(classpathFile);
	  }

	  /// <summary>
	  /// Returns the input stream of a file.
	  /// </summary>
	  /// <param name="file"> the <seealso cref="File"/> to load </param>
	  /// <returns> the file content as input stream </returns>
	  /// <exception cref="IoUtilException"> if the file cannot be loaded </exception>
	  public static Stream fileAsStream(File file)
	  {
		try
		{
		  return new BufferedInputStream(new FileStream(file, FileMode.Open, FileAccess.Read));
		}
		catch (FileNotFoundException e)
		{
		  throw LOG.fileNotFoundException(file.AbsolutePath, e);
		}
	  }

	  /// <summary>
	  /// Returns the <seealso cref="File"/> for a filename.
	  /// </summary>
	  /// <param name="filename"> the filename to load </param>
	  /// <returns> the file object </returns>
	  public static File getClasspathFile(string filename)
	  {
		if (string.ReferenceEquals(filename, null))
		{
		  throw LOG.nullParameter("filename");
		}

		return getClasspathFile(filename, null);
	  }

	  /// <summary>
	  /// Returns the <seealso cref="File"/> for a filename.
	  /// </summary>
	  /// <param name="filename"> the filename to load </param>
	  /// <param name="classLoader"> the classLoader to load file with, if null falls back to TCCL and then this class's classloader </param>
	  /// <returns> the file object </returns>
	  /// <exception cref="IoUtilException"> if the file cannot be loaded </exception>
	  public static File getClasspathFile(string filename, ClassLoader classLoader)
	  {
		if (string.ReferenceEquals(filename, null))
		{
		  throw LOG.nullParameter("filename");
		}

		URL fileUrl = null;

		if (classLoader != null)
		{
		  fileUrl = classLoader.getResource(filename);
		}
		if (fileUrl == null)
		{
		  // Try the current Thread context classloader
		  classLoader = Thread.CurrentThread.ContextClassLoader;
		  fileUrl = classLoader.getResource(filename);

		  if (fileUrl == null)
		  {
			// Finally, try the classloader for this class
			classLoader = typeof(IoUtil).ClassLoader;
			fileUrl = classLoader.getResource(filename);
		  }
		}

		if (fileUrl == null)
		{
		  throw LOG.fileNotFoundException(filename);
		}

		try
		{
		  return new File(fileUrl.toURI());
		}
		catch (URISyntaxException e)
		{
		  throw LOG.fileNotFoundException(filename, e);
		}
	  }

	}

}