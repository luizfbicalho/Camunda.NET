using System;
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
namespace org.camunda.bpm.engine.impl.util
{



	/// <summary>
	/// @author Tom Baeyens
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class IoUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  public static sbyte[] readInputStream(Stream inputStream, string inputStreamName)
	  {
		MemoryStream outputStream = new MemoryStream();
		sbyte[] buffer = new sbyte[16 * 1024];
		try
		{
		  int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
		  while (bytesRead != -1)
		  {
			outputStream.Write(buffer, 0, bytesRead);
			bytesRead = inputStream.Read(buffer, 0, buffer.Length);
		  }
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileReadingStream(inputStreamName, e);
		}
		return outputStream.toByteArray();
	  }

	  public static string readClasspathResourceAsString(string resourceName)
	  {
		Stream resourceAsStream = typeof(IoUtil).ClassLoader.getResourceAsStream(resourceName);

		if (resourceAsStream == null)
		{
		  throw new ProcessEngineException("resource " + resourceName + " not found");
		}

		MemoryStream outStream = new MemoryStream();

		int next;
		sbyte[] result;
		sbyte[] buffer = new sbyte[1024];

		BufferedInputStream inputStream = null;
		try
		{
		  inputStream = new BufferedInputStream(resourceAsStream);
		  while ((next = inputStream.read(buffer)) >= 0)
		  {
			outStream.Write(buffer, 0, next);
		  }

		  result = outStream.toByteArray();
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileReadingFile(resourceName, e);
		}
		finally
		{
		  IoUtil.closeSilently(inputStream);
		  IoUtil.closeSilently(outStream);
		}
		return StringHelper.NewString(result, Charset.forName("UTF-8"));
	  }

	  public static File getFile(string filePath)
	  {
		URL url = typeof(IoUtil).ClassLoader.getResource(filePath);
		try
		{
		  return new File(url.toURI());
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileGettingFile(filePath, e);
		}
	  }

	  public static void writeStringToFile(string content, string filePath)
	  {
		BufferedOutputStream outputStream = null;
		try
		{
		  outputStream = new BufferedOutputStream(new FileStream(getFile(filePath), FileMode.Create, FileAccess.Write));
		  outputStream.write(content.GetBytes());
		  outputStream.flush();
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileWritingToFile(filePath, e);
		}
		finally
		{
		  IoUtil.closeSilently(outputStream);
		}
	  }

	  /// <summary>
	  /// Closes the given stream. The same as calling <seealso cref="Closeable.close()"/>, but
	  /// errors while closing are silently ignored.
	  /// </summary>
	  public static void closeSilently(System.IDisposable closeable)
	  {
		try
		{
		  if (closeable != null)
		  {
			closeable.Dispose();
		  }
		}
		catch (IOException ignore)
		{
		  LOG.debugCloseException(ignore);
		}
	  }

	  /// <summary>
	  /// Flushes the given object. The same as calling <seealso cref="Flushable.flush()"/>, but
	  /// errors while flushing are silently ignored.
	  /// </summary>
	  public static void flushSilently(Flushable flushable)
	  {
		try
		{
		  if (flushable != null)
		  {
			flushable.flush();
		  }
		}
		catch (IOException ignore)
		{
		  LOG.debugCloseException(ignore);
		}
	  }
	}

}