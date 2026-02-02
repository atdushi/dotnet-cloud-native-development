# Creating an SSL Certificate Using `openssl`

## 0. Create or update the `conf` file to be used as input

The file you create or update should look something like this:

```conf
[req]
prompt             = no
default_bits       = 2048
distinguished_name = req_distinguished_name
req_extensions     = req_ext
x509_extensions    = v3_ca

[req_distinguished_name]
countryName                 = US
stateOrProvinceName         = Minnesota
localityName                = St. Paul
organizationName            = Carved Rock
organizationalUnitName      = Development
commonName                  = Carved Rock Local IdentityServer

[req_ext]
subjectAltName = @alt_names

[v3_ca]
subjectAltName = @alt_names

[alt_names]
DNS.1   = carvedrock.identity
```

## 1. Use `openssl` to create `crt` and `key` files

The command below references a `cr-id.conf` file that should already exist - with the contents from step 0 above.

```bash
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout cr-id-local.key -out cr-id-local.crt -config cr-id-local.conf
```

## 2. Export a `pfx` that you can use in an ASP.NET Core project

```bash
sudo openssl pkcs12 -export -out cr-id-local.pfx -inkey cr-id-local.key -in cr-id-local.crt
```

You will be prompted for an "export" password and will need to confirm the entry. This password
is needed when opening / using the `pfx` file.

I used `Learning1sGreat!` for the password in this example.

## 3. Trust the Certificate

### Create a HOSTS file entry for your DNS name

If you need to trust this certificate for use in a browser
on your localmachine, you need to create a `HOSTS` file
entry on your machine for the DNS name you specified in the
`conf` file above.  Something like this:

```hosts
127.0.0.1   carvedrock.identity  
```

### Import the `crt` File into Trusted Root Store

From Windows Explorer, navigate to the directory
containing the `crt` file, open it, and that should
bring up the Import Certificate wizard.  Import it
into the Trusted Root Authority store for the System
or the User.

### Add and Trust the `crt` file within Dockerfiles

For any Docker images that need to trust the new
certificate, add the following lines into a stage
that will be included in the final image:

```Dockerfile
COPY ../cert-creation/cr-id-local.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates
```

Note that the first location in the copy command
refers to the local `crt` file
that you created in the above commands.
