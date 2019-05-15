using System;
using System.Collections.Generic;
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
namespace org.camunda.bpm.container.impl.deployment.scanning
{

	using ProcessApplicationScanner = org.camunda.bpm.container.impl.deployment.scanning.spi.ProcessApplicationScanner;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;

	/// <summary>
	/// <para>Scans for bpmn20.xml files in the classpath of the given classloader.</para>
	/// 
	/// <para>Scans all branches of the classpath containing a META-INF/processes.xml
	/// file </para>
	/// 
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public class ClassPathProcessApplicationScanner : ProcessApplicationScanner
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public virtual IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string paResourceRootPath, URL metaFileUrl)
	  {
		return findResources(classLoader, paResourceRootPath, metaFileUrl, null);
	  }

	  public virtual IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string paResourceRootPath, URL metaFileUrl, string[] additionalResourceSuffixes)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, byte[]> resourceMap = new java.util.HashMap<String, byte[]>();
		IDictionary<string, sbyte[]> resourceMap = new Dictionary<string, sbyte[]>();

		// perform the scanning. (results are collected in 'resourceMap')
		scanPaResourceRootPath(classLoader, metaFileUrl, paResourceRootPath, additionalResourceSuffixes, resourceMap);

		return resourceMap;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void scanPaResourceRootPath(final ClassLoader classLoader, final java.net.URL metaFileUrl, final String paResourceRootPath, java.util.Map<String, byte[]> resourceMap)
	  public virtual void scanPaResourceRootPath(ClassLoader classLoader, URL metaFileUrl, string paResourceRootPath, IDictionary<string, sbyte[]> resourceMap)
	  {
		scanPaResourceRootPath(classLoader, metaFileUrl, paResourceRootPath, null, resourceMap);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void scanPaResourceRootPath(final ClassLoader classLoader, final java.net.URL metaFileUrl, final String paResourceRootPath, String[] additionalResourceSuffixes, java.util.Map<String, byte[]> resourceMap)
	  public virtual void scanPaResourceRootPath(ClassLoader classLoader, URL metaFileUrl, string paResourceRootPath, string[] additionalResourceSuffixes, IDictionary<string, sbyte[]> resourceMap)
	  {

		if (!string.ReferenceEquals(paResourceRootPath, null) && !paResourceRootPath.StartsWith("pa:", StringComparison.Ordinal))
		{

		  //  1. CASE: paResourceRootPath specified AND it is a "classpath:" resource root

		  // "classpath:directory" -> "directory"
		  string strippedPath = paResourceRootPath.Replace("classpath:", "");
		  // "directory" -> "directory/"
		  strippedPath = strippedPath.EndsWith("/", StringComparison.Ordinal) ? strippedPath : strippedPath + "/";
		  IEnumerator<URL> resourceRoots = loadClasspathResourceRoots(classLoader, strippedPath);

		  while (resourceRoots.MoveNext())
		  {
			URL resourceRoot = resourceRoots.Current;
			scanUrl(resourceRoot, strippedPath, false, additionalResourceSuffixes, resourceMap);
		  }


		}
		else
		{

		  // 2nd. CASE: no paResourceRootPath specified OR paResourceRootPath is PA-local

		  string strippedPaResourceRootPath = null;
		  if (!string.ReferenceEquals(paResourceRootPath, null))
		  {
			// "pa:directory" -> "directory"
			strippedPaResourceRootPath = paResourceRootPath.Replace("pa:", "");
			// "directory" -> "directory/"
			strippedPaResourceRootPath = strippedPaResourceRootPath.EndsWith("/", StringComparison.Ordinal) ? strippedPaResourceRootPath : strippedPaResourceRootPath + "/";
		  }

		  scanUrl(metaFileUrl, strippedPaResourceRootPath, true, additionalResourceSuffixes, resourceMap);

		}
	  }

	  protected internal virtual void scanUrl(URL url, string paResourceRootPath, bool isPaLocal, string[] additionalResourceSuffixes, IDictionary<string, sbyte[]> resourceMap)
	  {

		string urlPath = url.toExternalForm();

		if (isPaLocal)
		{

		  if (urlPath.StartsWith("file:", StringComparison.Ordinal) || urlPath.StartsWith("jar:", StringComparison.Ordinal) || urlPath.StartsWith("wsjar:", StringComparison.Ordinal) || urlPath.StartsWith("zip:", StringComparison.Ordinal))
		  {
			urlPath = url.Path;
			int withinArchive = urlPath.IndexOf('!');
			if (withinArchive != -1)
			{
			  urlPath = urlPath.Substring(0, withinArchive);
			}
			else
			{
			  File file = new File(urlPath);
			  urlPath = file.ParentFile.Parent;
			}
		  }

		}
		else
		{
		  if (urlPath.StartsWith("file:", StringComparison.Ordinal) || urlPath.StartsWith("jar:", StringComparison.Ordinal) || urlPath.StartsWith("wsjar:", StringComparison.Ordinal) || urlPath.StartsWith("zip:", StringComparison.Ordinal))
		  {
			urlPath = url.Path;
			int withinArchive = urlPath.IndexOf('!');
			if (withinArchive != -1)
			{
			  urlPath = urlPath.Substring(0, withinArchive);
			}
		  }

		}

		try
		{
		  urlPath = URLDecoder.decode(urlPath, "UTF-8");
		}
		catch (UnsupportedEncodingException e)
		{
		  throw LOG.cannotDecodePathName(e);
		}

		LOG.debugRootPath(urlPath);

		scanPath(urlPath, paResourceRootPath, isPaLocal, additionalResourceSuffixes, resourceMap);

	  }

	  protected internal virtual void scanPath(string urlPath, string paResourceRootPath, bool isPaLocal, string[] additionalResourceSuffixes, IDictionary<string, sbyte[]> resourceMap)
	  {
		if (urlPath.StartsWith("file:", StringComparison.Ordinal))
		{
		  urlPath = urlPath.Substring(5);
		}
		if (urlPath.IndexOf('!') > 0)
		{
		  urlPath = urlPath.Substring(0, urlPath.IndexOf('!'));
		}

		File file = new File(urlPath);
		if (file.Directory)
		{
		  string path = file.Path;
		  string rootPath = path.EndsWith(File.separator, StringComparison.Ordinal) ? path : path + File.separator;
		  handleDirectory(file, rootPath, paResourceRootPath, paResourceRootPath, isPaLocal, additionalResourceSuffixes, resourceMap);
		}
		else
		{
		  handleArchive(file, paResourceRootPath, additionalResourceSuffixes, resourceMap);
		}
	  }

	  protected internal virtual void handleArchive(File file, string paResourceRootPath, string[] additionalResourceSuffixes, IDictionary<string, sbyte[]> resourceMap)
	  {
		try
		{
		  ZipFile zipFile = new ZipFile(file);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Iterator< ? extends java.util.zip.ZipEntry> entries = zipFile.entries();
		  IEnumerator<ZipEntry> entries = zipFile.entries();
		  while (entries.MoveNext())
		  {
			ZipEntry zipEntry = entries.Current;
			string modelFileName = zipEntry.Name;
			if (ProcessApplicationScanningUtil.isDeployable(modelFileName, additionalResourceSuffixes) && isBelowPath(modelFileName, paResourceRootPath))
			{
			  string resourceName = modelFileName;
			  if (!string.ReferenceEquals(paResourceRootPath, null) && paResourceRootPath.Length > 0)
			  {
				// "directory/sub_directory/process.bpmn" -> "sub_directory/process.bpmn"
				resourceName = modelFileName.replaceFirst(paResourceRootPath, "");
			  }
			  addResource(zipFile.getInputStream(zipEntry), resourceMap, file.Name + "!", resourceName);
			  // find diagram(s) for process
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Iterator< ? extends java.util.zip.ZipEntry> entries2 = zipFile.entries();
			  IEnumerator<ZipEntry> entries2 = zipFile.entries();
			  while (entries2.MoveNext())
			  {
				ZipEntry zipEntry2 = entries2.Current;
				string diagramFileName = zipEntry2.Name;
				if (ProcessApplicationScanningUtil.isDiagram(diagramFileName, modelFileName))
				{
				  if (!string.ReferenceEquals(paResourceRootPath, null) && paResourceRootPath.Length > 0)
				  {
					// "directory/sub_directory/process.png" -> "sub_directory/process.png"
					diagramFileName = diagramFileName.replaceFirst(paResourceRootPath, "");
				  }
				  addResource(zipFile.getInputStream(zipEntry), resourceMap, file.Name + "!", diagramFileName);
				}
			  }
			}
		  }
		  zipFile.close();
		}
		catch (IOException e)
		{
		  throw LOG.exceptionWhileScanning(file.AbsolutePath, e);
		}
	  }

	  protected internal virtual void handleDirectory(File directory, string rootPath, string localPath, string paResourceRootPath, bool isPaLocal, string[] additionalResourceSuffixes, IDictionary<string, sbyte[]> resourceMap)
	  {
		File[] paths = directory.listFiles();

		string currentPathSegment = localPath;
		if (!string.ReferenceEquals(localPath, null) && localPath.Length > 0)
		{
		  if (localPath.IndexOf('/') > 0)
		  {
			currentPathSegment = localPath.Substring(0, localPath.IndexOf('/'));
			localPath = StringHelper.SubstringSpecial(localPath, localPath.IndexOf('/') + 1, localPath.Length);
		  }
		  else
		  {
			localPath = null;
		  }
		}

		foreach (File path in paths)
		{


		  if (isPaLocal && !string.ReferenceEquals(currentPathSegment, null) && currentPathSegment.Length > 0)
		  {

			if (path.Directory)
			{
			  // only descend into directory, if below resource root:
			  if (path.Name.Equals(currentPathSegment))
			  {
				handleDirectory(path, rootPath, localPath, paResourceRootPath, isPaLocal, additionalResourceSuffixes, resourceMap);
			  }
			}

		  }
		  else
		  { // at resource root or below -> continue scanning
			string modelFileName = path.Path;
			if (!path.Directory && ProcessApplicationScanningUtil.isDeployable(modelFileName, additionalResourceSuffixes))
			{
			  // (1): "...\directory\sub_directory\process.bpmn" -> "sub_directory\process.bpmn"
			  // (2): "sub_directory\process.bpmn" -> "sub_directory/process.bpmn"
			  addResource(path, resourceMap, paResourceRootPath, modelFileName.Replace(rootPath, "").Replace("\\", "/"));
			  // find diagram(s) for process
			  foreach (File file in paths)
			  {
				string diagramFileName = file.Path;
				if (!path.Directory && ProcessApplicationScanningUtil.isDiagram(diagramFileName, modelFileName))
				{
				  // (1): "...\directory\sub_directory\process.png" -> "sub_directory\process.png"
				  // (2): "sub_directory\process.png" -> "sub_directory/process.png"
				  addResource(file, resourceMap, paResourceRootPath, diagramFileName.Replace(rootPath, "").Replace("\\", "/"));
				}
			  }
			}
			else if (path.Directory)
			{
			  handleDirectory(path, rootPath, localPath, paResourceRootPath, isPaLocal, additionalResourceSuffixes, resourceMap);
			}
		  }
		}
	  }

	  protected internal virtual void addResource(object source, IDictionary<string, sbyte[]> resourceMap, string resourceRootPath, string resourceName)
	  {

		string resourcePath = (string.ReferenceEquals(resourceRootPath, null) ? "" : resourceRootPath).concat(resourceName);

		LOG.debugDiscoveredResource(resourcePath);

		Stream inputStream = null;

		try
		{
		  if (source is File)
		  {
			try
			{
			  inputStream = new FileStream((File) source, FileMode.Open, FileAccess.Read);
			}
			catch (IOException e)
			{
			  throw LOG.cannotOpenFileInputStream(((File) source).AbsolutePath, e);
			}
		  }
		  else
		  {
			inputStream = (Stream) source;
		  }
		  sbyte[] bytes = IoUtil.readInputStream(inputStream, resourcePath);

		  resourceMap[resourceName] = bytes;

		}
		finally
		{
		  if (inputStream != null)
		  {
			IoUtil.closeSilently(inputStream);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.Iterator<java.net.URL> loadClasspathResourceRoots(final ClassLoader classLoader, String strippedPaResourceRootPath)
	  protected internal virtual IEnumerator<URL> loadClasspathResourceRoots(ClassLoader classLoader, string strippedPaResourceRootPath)
	  {
		IEnumerator<URL> resourceRoots;
		try
		{
		  resourceRoots = classLoader.getResources(strippedPaResourceRootPath);
		}
		catch (IOException e)
		{
		  throw LOG.couldNotGetResource(strippedPaResourceRootPath, classLoader, e);
		}
		return resourceRoots;
	  }

	  protected internal virtual bool isBelowPath(string processFileName, string paResourceRootPath)
	  {
		if (string.ReferenceEquals(paResourceRootPath, null) || paResourceRootPath.Length == 0)
		{
		  return true;
		}
		else
		{
		  return processFileName.StartsWith(paResourceRootPath, StringComparison.Ordinal);
		}
	  }

	}

}