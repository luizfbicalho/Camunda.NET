using System;

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
namespace org.camunda.bpm.engine.rest.util.container
{

	using LifecycleException = org.apache.catalina.LifecycleException;
	using Tomcat = org.apache.catalina.startup.Tomcat;
	using ProcessEngineProvider = org.camunda.bpm.engine.rest.spi.ProcessEngineProvider;
	using MockedProcessEngineProvider = org.camunda.bpm.engine.rest.spi.impl.MockedProcessEngineProvider;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using ClassLoaderAsset = org.jboss.shrinkwrap.api.asset.ClassLoaderAsset;
	using ZipExporter = org.jboss.shrinkwrap.api.exporter.ZipExporter;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Maven = org.jboss.shrinkwrap.resolver.api.maven.Maven;
	using PomEquippedResolveStage = org.jboss.shrinkwrap.resolver.api.maven.PomEquippedResolveStage;
	using ScopeType = org.jboss.shrinkwrap.resolver.api.maven.ScopeType;
	using MavenDependencies = org.jboss.shrinkwrap.resolver.api.maven.coordinate.MavenDependencies;


	public abstract class TomcatServerBootstrap : EmbeddedServerBootstrap
	{

	  private Tomcat tomcat;
	  private string workingDir;
	  private string webXmlPath;

	  public TomcatServerBootstrap(string webXmlPath)
	  {
		this.webXmlPath = webXmlPath;
	  }

	  public override void start()
	  {
		Properties serverProperties = readProperties();
		int port = int.Parse(serverProperties.getProperty(PORT_PROPERTY));

		tomcat = new Tomcat();
		tomcat.Port = port;
		tomcat.BaseDir = WorkingDir;

		tomcat.Host.AppBase = WorkingDir;
		tomcat.Host.AutoDeploy = true;
		tomcat.Host.DeployOnStartup = true;

		string contextPath = "/" + ContextPath;

		PomEquippedResolveStage resolver = Maven.configureResolver().useLegacyLocalRepo(true).workOffline().loadPomFromFile("pom.xml");

		WebArchive wa = ShrinkWrap.create(typeof(WebArchive), "rest-test.war").setWebXML(webXmlPath).addAsLibraries(resolver.resolve("org.codehaus.jackson:jackson-jaxrs:1.6.5").withTransitivity().asFile()).addAsLibraries(resolver.addDependencies(MavenDependencies.createDependency("org.mockito:mockito-core", ScopeType.TEST, false, MavenDependencies.createExclusion("org.hamcrest:hamcrest-core"))).resolve().withTransitivity().asFile()).addAsServiceProvider(typeof(ProcessEngineProvider), typeof(MockedProcessEngineProvider)).add(new ClassLoaderAsset("runtime/tomcat/context.xml"), "META-INF/context.xml").addPackages(true, "org.camunda.bpm.engine.rest");

		addRuntimeSpecificLibraries(wa, resolver);
		wa.WebXML = webXmlPath;

		string webAppPath = WorkingDir + "/" + ContextPath + ".war";

		wa.@as(typeof(ZipExporter)).exportTo(new File(webAppPath), true);

		tomcat.addWebapp(tomcat.Host, contextPath, webAppPath);

		try
		{
		  tomcat.start();
		}
		catch (LifecycleException e)
		{
		  throw new Exception(e);
		}
	  }

	  protected internal abstract void addRuntimeSpecificLibraries(WebArchive wa, PomEquippedResolveStage resolver);

	  private string ContextPath
	  {
		  get
		  {
			return "rest-test";
		  }
	  }

	  public override void stop()
	  {
		try
		{
		  try
		  {
			tomcat.stop();
		  }
		  catch (Exception e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Logger.getLogger(this.GetType().FullName).log(Level.WARNING, "Failed to stop tomcat instance", e);
		  }

		  try
		  {
			tomcat.destroy();
		  }
		  catch (Exception e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Logger.getLogger(this.GetType().FullName).log(Level.WARNING, "Failed to destroy instance", e);
		  }

		  tomcat = null;
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
	  }

	  public virtual string WorkingDir
	  {
		  get
		  {
			if (string.ReferenceEquals(workingDir, null))
			{
			  workingDir = System.getProperty("java.io.tmpdir");
			}
			return workingDir;
		  }
		  set
		  {
			this.workingDir = value;
		  }
	  }


	}

}