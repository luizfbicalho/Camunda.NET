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
	using VFS = org.jboss.vfs.VFS;
	using VirtualFile = org.jboss.vfs.VirtualFile;
	using VirtualFileFilter = org.jboss.vfs.VirtualFileFilter;

	/// <summary>
	/// <para>A <seealso cref="ProcessArchiveScanner"/> which uses Jboss VFS for
	/// scanning the process archive for processes.</para>
	/// 
	/// <para>This implementation should be used on Jboss AS 7</para>
	/// 
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public class VfsProcessApplicationScanner : ProcessApplicationScanner
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  public virtual IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string resourceRootPath, URL processesXml)
	  {
		return findResources(classLoader, resourceRootPath, processesXml, null);
	  }

	  public virtual IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string resourceRootPath, URL processesXml, string[] additionalResourceSuffixes)
	  {

		// the map in which we collect the resources
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, byte[]> resources = new java.util.HashMap<String, byte[]>();
		IDictionary<string, sbyte[]> resources = new Dictionary<string, sbyte[]>();

		if (!string.ReferenceEquals(resourceRootPath, null) && !resourceRootPath.StartsWith("pa:", StringComparison.Ordinal))
		{

			//  1. CASE: paResourceRootPath specified AND it is a "classpath:" resource root

			string strippedPath = resourceRootPath.Replace("classpath:", "");
			IEnumerator<URL> resourceRoots = loadClasspathResourceRoots(classLoader, strippedPath);

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (!resourceRoots.hasMoreElements())
			{
			  LOG.cannotFindResourcesForPath(resourceRootPath, classLoader);
			}

			while (resourceRoots.MoveNext())
			{
			  URL resourceRoot = resourceRoots.Current;
			  VirtualFile virtualRoot = getVirtualFileForUrl(resourceRoot);
			  scanRoot(virtualRoot, additionalResourceSuffixes, resources);
			}

		}
		else
		{

		  // 2nd. CASE: no paResourceRootPath specified OR paResourceRootPath is PA-local
		  if (processesXml != null)
		  {

			VirtualFile virtualFile = getVirtualFileForUrl(processesXml);
			// use the parent resource of the META-INF folder
			VirtualFile resourceRoot = virtualFile.Parent.Parent;

			if (!string.ReferenceEquals(resourceRootPath, null))
			{ // pa-local path provided
			  string strippedPath = resourceRootPath.Replace("pa:", "");
			  resourceRoot = resourceRoot.getChild(strippedPath);
			}

			// perform the scanning
			scanRoot(resourceRoot, additionalResourceSuffixes, resources);

		  }
		}

		return resources;

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.jboss.vfs.VirtualFile getVirtualFileForUrl(final java.net.URL url)
	  protected internal virtual VirtualFile getVirtualFileForUrl(URL url)
	  {
		try
		{
		  return VFS.getChild(url.toURI());
		}
		catch (URISyntaxException e)
		{
		  throw LOG.exceptionWhileGettingVirtualFolder(url, e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void scanRoot(org.jboss.vfs.VirtualFile processArchiveRoot, final String[] additionalResourceSuffixes, java.util.Map<String, byte[]> resources)
	  protected internal virtual void scanRoot(VirtualFile processArchiveRoot, string[] additionalResourceSuffixes, IDictionary<string, sbyte[]> resources)
	  {
		try
		{
		  IList<VirtualFile> processes = processArchiveRoot.getChildrenRecursively(new VirtualFileFilterAnonymousInnerClass(this, additionalResourceSuffixes));
		  foreach (VirtualFile process in processes)
		  {
			addResource(process, processArchiveRoot, resources);
			// find diagram(s) for process
			IList<VirtualFile> diagrams = process.Parent.getChildren(new VirtualFileFilterAnonymousInnerClass2(this));
			foreach (VirtualFile diagram in diagrams)
			{
			  addResource(diagram, processArchiveRoot, resources);
			}
		  }
		}
		catch (IOException e)
		{
		  LOG.cannotScanVfsRoot(processArchiveRoot, e);
		}
	  }

	  private class VirtualFileFilterAnonymousInnerClass : VirtualFileFilter
	  {
		  private readonly VfsProcessApplicationScanner outerInstance;

		  private string[] additionalResourceSuffixes;

		  public VirtualFileFilterAnonymousInnerClass(VfsProcessApplicationScanner outerInstance, string[] additionalResourceSuffixes)
		  {
			  this.outerInstance = outerInstance;
			  this.additionalResourceSuffixes = additionalResourceSuffixes;
		  }

		  public bool accepts(VirtualFile file)
		  {
			return file.File && ProcessApplicationScanningUtil.isDeployable(file.Name, additionalResourceSuffixes);
		  }
	  }

	  private class VirtualFileFilterAnonymousInnerClass2 : VirtualFileFilter
	  {
		  private readonly VfsProcessApplicationScanner outerInstance;

		  public VirtualFileFilterAnonymousInnerClass2(VfsProcessApplicationScanner outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public bool accepts(VirtualFile file)
		  {
			return ProcessApplicationScanningUtil.isDiagram(file.Name, process.Name);
		  }
	  }

	  private void addResource(VirtualFile virtualFile, VirtualFile processArchiveRoot, IDictionary<string, sbyte[]> resources)
	  {
		string resourceName = virtualFile.getPathNameRelativeTo(processArchiveRoot);
		try
		{
		  Stream inputStream = virtualFile.openStream();
		  sbyte[] bytes = IoUtil.readInputStream(inputStream, resourceName);
		  IoUtil.closeSilently(inputStream);
		  resources[resourceName] = bytes;
		}
		catch (IOException e)
		{
		  LOG.cannotReadInputStreamForFile(resourceName, processArchiveRoot, e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.Iterator<java.net.URL> loadClasspathResourceRoots(final ClassLoader classLoader, String strippedPaResourceRootPath)
	  protected internal virtual IEnumerator<URL> loadClasspathResourceRoots(ClassLoader classLoader, string strippedPaResourceRootPath)
	  {
		try
		{
		  return classLoader.getResources(strippedPaResourceRootPath);
		}
		catch (IOException e)
		{
		  throw LOG.exceptionWhileLoadingCpRoots(strippedPaResourceRootPath, classLoader, e);
		}
	  }

	}

}